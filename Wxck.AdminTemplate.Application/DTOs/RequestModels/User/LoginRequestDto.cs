using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wxck.AdminTemplate.CommsCore.Enums.User;
using Wxck.AdminTemplate.Application.Validators.Attributes.User;

namespace Wxck.AdminTemplate.Application.DTOs.RequestModels.User {

    public class LoginRequestDto : BaseCaptchaRequestDto {

        /// <summary>
        /// 登录Code,可以是用户名、用户Code、邮箱等
        /// </summary>
        [Required(ErrorMessage = "登录Code不能为空")]
        public string? LoginCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "密码不能为空!"),
         MaxLength(20, ErrorMessage = "密码长度不能超过20个字符"),
         MinLength(8, ErrorMessage = "密码长度不能小于8个字符"),
         RegularExpression("^[^\\u4e00-\\u9fa5\\s]*$", ErrorMessage = "密码不能包含中文、空格或换行")]
        public string? PassWord { get; set; } = string.Empty;

        /// <summary>
        /// 手机号
        /// </summary>
        [RegularExpression("^1[3-9]\\d{9}$", ErrorMessage = "手机号格式错误"),
         UserPhoneExists(IsExists = true, ErrorMessage = "该手机号未注册")]
        public string? Phone { get; set; }

        /// <summary>
        /// 第三方平台Token
        /// </summary>
        public string? PlatformToken { get; set; }

        /// <summary>
        /// 登录方式
        /// </summary>
        [LoginMethodValid]
        public LoginMethod LoginMethod { get; set; } = LoginMethod.LoginCode;
    }
}