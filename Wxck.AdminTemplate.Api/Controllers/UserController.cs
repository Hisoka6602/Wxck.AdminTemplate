using Microsoft.AspNetCore.Mvc;
using Wxck.AdminTemplate.Api.Filter;
using Wxck.AdminTemplate.Application;
using Microsoft.AspNetCore.Authorization;
using Wxck.AdminTemplate.CommsCore.Enums.User;

namespace Wxck.AdminTemplate.Api.Controllers {

    [Route("api/[controller]")]
    [ApiController, Produces("application/json")]
    public class UserController : ControllerBase {
        /*/// <summary>
        /// 个人信息
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("Info"), Authorize, UserStatus(Status = UserStatus.Active)]
        public async Task<JsonResult> Info(CancellationToken cancellationToken) {
            return JsonResultDto.Fail("未实现");
        }*/
    }
}