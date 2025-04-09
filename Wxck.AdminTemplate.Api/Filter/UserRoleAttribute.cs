using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wxck.AdminTemplate.CommsCore.Enums.User;
using Wxck.AdminTemplate.Domain.Repositories.User;

namespace Wxck.AdminTemplate.Api.Filter {

    public class UserRoleAttribute : ActionFilterAttribute {
        public UserRole Role { get; set; }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next) {
            //获取Token

            var userCode = context.HttpContext.Response.HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(userCode)) {
                context.Result = await Task.FromResult(new JsonResult(new { Result = false, Msg = "Token错误!" }) { StatusCode = 200 });
                return;
            }

            var userRepository = context.HttpContext.RequestServices.GetService<IUserRepository>();
            if (userRepository is not null) {
                var licenseUserInfo = userRepository?.MemoryCacheData()?.
                    ConfigureAwait(false).GetAwaiter().GetResult()
                    ?.FirstOrDefault(f => f.UserCode.Equals(userCode));

                if (licenseUserInfo is null) {
                    context.Result = await Task.FromResult(new JsonResult(new { Result = false, Msg = "账号不存在!" }) { StatusCode = 200 });
                    return;
                }
                else {
                    if (!Role.HasFlag(licenseUserInfo.Role)) {
                        context.Result = await Task.FromResult(new JsonResult(new { Result = false, Msg = "您无权限访问!" }) { StatusCode = 200 });
                        return;
                    }
                }
            }
            else {
                context.Result = await Task.FromResult(new JsonResult(new { Result = false, Msg = "系统配置错误!" }) { StatusCode = 200 });
                return;
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}