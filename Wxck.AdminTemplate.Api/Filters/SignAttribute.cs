using Wxck.AdminTemplate.Application;
using Microsoft.AspNetCore.Mvc.Filters;
using Wxck.AdminTemplate.CrossCutting.Security.Signing;

namespace Wxck.AdminTemplate.Api.Filter {

    /// <summary>
    /// Sign校验
    /// </summary>
    public class SignAttribute : ActionFilterAttribute {

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next) {
            var arguments = context.ActionArguments;
            var any = arguments.Any(a => a.Value != null && a.Value.GetType() == typeof(FormFile));
            if (any) {
                await base.OnActionExecutionAsync(context, next);
                return;
            }
            var sign = context.HttpContext.Request.Headers["Sign"];
            if (string.IsNullOrEmpty(sign)) {
                context.Result = JsonResultDto.Fail("Sign不能为空!");
                return;
            }

            var service = context.HttpContext.RequestServices.GetService<ISignProvider>();
            if (context.HttpContext.Request.Method.Equals("POST")) {
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                var result = await new StreamReader(context.HttpContext.Request.Body)?.ReadToEndAsync()!;
                var controller = context.RouteData.Values["controller"]?.ToString();
                var isValid = service?.IsValid(sign!, controller!, result);
                if (isValid == true) {
                    await base.OnActionExecutionAsync(context, next);
                    return;
                }
            }
            else {
                var controller = context.RouteData.Values["controller"]?.ToString();
                var isValid = service?.IsValid(sign!, controller!, "");
                if (isValid == true) {
                    await base.OnActionExecutionAsync(context, next);
                    return;
                }
            }
            context.Result = JsonResultDto.Fail("Sign效验错误!");
        }
    }
}