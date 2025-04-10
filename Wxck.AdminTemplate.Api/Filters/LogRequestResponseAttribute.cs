using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Wxck.AdminTemplate.Domain.Entities.Logs;
using Wxck.AdminTemplate.Domain.Repositories.Logs;
using Wxck.AdminTemplate.Domain.Repositories.User;

namespace Wxck.AdminTemplate.Api.Filters {

    public class LogRequestResponseAttribute : ActionFilterAttribute {
        private readonly ILogger<LogRequestResponseAttribute> _logger;
        private readonly IOperationLogRepository _operationLogRepository;
        private readonly IUserRepository _userRepository;

        public LogRequestResponseAttribute(ILogger<LogRequestResponseAttribute> logger,
            IOperationLogRepository operationLogRepository,
            IUserRepository userRepository) {
            _logger = logger;
            _operationLogRepository = operationLogRepository;
            _userRepository = userRepository;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next) {
            var request = context.HttpContext.Request;
            var requestBody = await ReadRequestBodyAsync(request);
            // 获取操作描述（如果存在）
            var operationDescription = context.HttpContext.Items["OperationDescription"]?.ToString() ?? "未定义的操作";
            // 将请求数据存储到 context.Items
            context.HttpContext.Items["RequestContent"] = requestBody;
            context.HttpContext.Items["RequestMethod"] = request.Method;
            var userCode = context.HttpContext.Response.HttpContext.User.Identity?.Name;

            var memoryCacheData = await _userRepository.MemoryCacheData();
            var userInfo = memoryCacheData?.FirstOrDefault(f => f.UserCode.Equals(userCode));
            var stopwatch = Stopwatch.StartNew();  // 启动计时器
            // 执行请求管道
            var resultContext = await next(); // 继续执行请求
            stopwatch.Stop();  // 停止计时
            // 在响应执行后获取响应内容
            var response = context.HttpContext.Response;

            var responseBody = ReadResponseBodyAsync(resultContext);

            // var responseBody = await ReadResponseBodyAsync(response);

            // 获取请求内容和响应内容
            var requestContent = context.HttpContext.Items["RequestContent"]?.ToString() ?? string.Empty;
            // var requestMethod = context.HttpContext.Items["RequestMethod"]?.ToString() ?? string.Empty;
            var requestMethod = context.ActionDescriptor is ControllerActionDescriptor actionDescriptor
                ? actionDescriptor.ActionName
                : string.Empty;
            var operationLog = new OperationLogInfoModel() {
                RequestContent = requestContent,
                ResponseContent = responseBody,
                OperationDescription = operationDescription,
                IsSuccessful = response.StatusCode == 200,
                RequestIp = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                OperatorId = userInfo?.Id ?? 0,
                OperationTime = DateTime.Now,
                RequestMethod = requestMethod,
                TimeSpent = stopwatch.ElapsedMilliseconds
            };

            // 存储日志到数据库
            var insert = await _operationLogRepository.Insert(operationLog);
            if (!insert) {
                _logger.LogError($"记录操作日志失败:{JsonConvert.SerializeObject(operationLog, Formatting.Indented)}");
            }
        }

        // 读取请求体
        private async Task<string> ReadRequestBodyAsync(Microsoft.AspNetCore.Http.HttpRequest request) {
            if (request.ContentType?.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase) == true ||
                request.ContentType?.Contains("image", StringComparison.OrdinalIgnoreCase) == true) {
                return string.Empty;
            }

            request.EnableBuffering();
            request.Body.Position = 0;
            using var ms = new MemoryStream();
            await request.Body.CopyToAsync(ms);
            var b = ms.ToArray();
            var body = Encoding.UTF8.GetString(b);
            /*using var reader = new StreamReader(request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);*/
            return body;
        }

        // 读取响应体
        private string ReadResponseBodyAsync(ActionExecutedContext resultContext) {
            return resultContext.Result switch {
                ObjectResult objectResult => JsonConvert.SerializeObject(objectResult.Value),
                JsonResult jsonResult => JsonConvert.SerializeObject(jsonResult.Value),
                ContentResult contentResult => contentResult.Content ?? string.Empty,
                _ => string.Empty
            };
        }
    }
}