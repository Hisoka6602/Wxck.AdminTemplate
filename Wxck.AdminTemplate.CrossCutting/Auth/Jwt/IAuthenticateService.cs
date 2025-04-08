using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wxck.AdminTemplate.CrossCutting.Auth.Jwt {

    public interface IAuthenticateService {

        bool IsAuthenticated(LoginRequestDto request, out string token);

        /// <summary>
        /// 检查 Token 是否在黑名单中
        /// </summary>
        /// <param name="token">待检查的 JWT</param>
        /// <returns>是否被黑名单标记</returns>
        bool IsTokenBlacklisted(string token);
    }

    public class LoginRequestDto {
        /// <summary>
        /// 账号
        /// </summary>

        [Required, Description("账号")]
        public string? UserCode { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required, Description("密码")]
        public string? PassWord { get; set; }
    }
}