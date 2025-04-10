using System.Threading;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Mvc;
using Wxck.AdminTemplate.Api.Filter;
using Wxck.AdminTemplate.Application;
using Microsoft.AspNetCore.Authorization;
using Wxck.AdminTemplate.CommsCore.Enums.User;
using Wxck.AdminTemplate.Application.Services.User;
using Wxck.AdminTemplate.Application.DTOs.RequestModels.User;

namespace Wxck.AdminTemplate.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController, Produces("application/json")]
    public class UserController : ControllerBase {
        private readonly ICaptcha _captcha;
        private readonly IUserService _userService;

        public UserController(ICaptcha captcha,
            IUserService userService) {
            _captcha = captcha;
            _userService = userService;
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
        /// 注册
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public async Task<JsonResult> Register([FromBody] RegisterRequestDto dto, CancellationToken cancellationToken) {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var (key, value) = await _userService.Register(dto.UserCode, dto.UserName, dto.PassWord, ip, cancellationToken);
            return key ? JsonResultDto.Success(value?.ToString() ?? string.Empty)
                : JsonResultDto.Fail(value?.ToString() ?? string.Empty);
        }

        [HttpPost("Login")]
        public async Task<JsonResult> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken) {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var (key, value) = await _userService.Login(dto.LoginCode ?? string.Empty, dto.PassWord ?? string.Empty, dto.Phone,
                dto.PlatformToken, dto.LoginMethod, ip, cancellationToken);

            return key ? JsonResultDto.Success("登录成功", value)
                : JsonResultDto.Fail(value?.ToString() ?? string.Empty);
        }

        /// <summary>
        /// 个人信息
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("Info"), Authorize, UserStatus(Status = UserStatus.Active)]
        public async Task<JsonResult> Info(CancellationToken cancellationToken) {
            var code = HttpContext.Response.HttpContext.User.Identity?.Name;
            var (key, value) = await _userService.Info(code, cancellationToken);
            return key ? JsonResultDto.Success("查询成功", value)
                : JsonResultDto.Fail(value?.ToString() ?? string.Empty);
        }

        //登录
        //修改密码
        //获取验证码
    }
}