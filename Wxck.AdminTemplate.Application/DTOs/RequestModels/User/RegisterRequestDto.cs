using System.ComponentModel.DataAnnotations;
using Wxck.AdminTemplate.Application.Validators.Attributes.User;

namespace Wxck.AdminTemplate.Application.DTOs.RequestModels.User {
    public class RegisterRequestDto : BaseCaptchaRequestDto {
        /// <summary>
        /// 用户代码
        /// </summary>

        [Required(ErrorMessage = "用户代码不能为空!"),
         UserCodeExists(IsExists = false, ErrorMessage = "该账号已注册!"),
         MaxLength(15, ErrorMessage = "代码长度不能超过15个字符"),
         RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "只允许字母和数字,并且开头必须为字母")]
        public string UserCode { get; set; } = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空!"),
         MinLength(2, ErrorMessage = "名称长度不能小于2个字符"),
         MaxLength(15, ErrorMessage = "名称长度不能超过15个字符"),
         UserNameExists(IsExists = false, ErrorMessage = "当前用户名已存在!")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空!"),
         MaxLength(20, ErrorMessage = "密码长度不能超过20个字符"),
         MinLength(8, ErrorMessage = "密码长度不能小于8个字符"),
         RegularExpression("^[^\\u4e00-\\u9fa5\\s]*$", ErrorMessage = "密码不能包含中文、空格或换行")]
        public string PassWord { get; set; } = string.Empty;
    }
}