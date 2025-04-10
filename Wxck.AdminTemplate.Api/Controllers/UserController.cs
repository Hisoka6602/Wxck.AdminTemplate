using System.Threading;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Mvc;
using Wxck.AdminTemplate.Api.Filter;
using Wxck.AdminTemplate.Application;
using Microsoft.AspNetCore.Authorization;
using Wxck.AdminTemplate.CommsCore.Enums.User;

namespace Wxck.AdminTemplate.Api.Controllers {

    [Route("api/[controller]")]
    [ApiController, Produces("application/json")]
    public class UserController : ControllerBase {
        private readonly ICaptcha _captcha;

        public UserController(ICaptcha captcha) {
            _captcha = captcha;
        }

        /// <summary>
        /// 获取图形验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCaptcha")]
        public async Task<JsonResult> GetCaptcha() {
            await Task.Yield();
            var unixTimeMilliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var captchaData = _captcha.Generate(unixTimeMilliseconds.ToString());
            return JsonResultDto.Success("验证码获取成功",
                new { Id = unixTimeMilliseconds, Img = captchaData.Base64 });
        }

        /// <summary>
        /// 个人信息
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("Info"), Authorize, UserStatus(Status = UserStatus.Active)]
        public async Task<JsonResult> Info(CancellationToken cancellationToken) {
            await Task.Delay(1000, cancellationToken);
            return JsonResultDto.Fail("未实现");
        }

        //注册
        //登录
        //修改密码
        //获取验证码
    }
}