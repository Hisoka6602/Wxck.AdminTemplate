using System.ComponentModel.DataAnnotations;
using Wxck.AdminTemplate.CommsCore.Enums.User;
using Wxck.AdminTemplate.Application.DTOs.RequestModels.User;

namespace Wxck.AdminTemplate.Application.Validators.Attributes.User {

    public class LoginMethodValidAttribute : ValidationAttribute {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
            if (value is LoginMethod method &&
                validationContext.ObjectInstance is LoginRequestDto loginRequest) {
                if (method == LoginMethod.LoginCode) {
                    if (string.IsNullOrEmpty(loginRequest.LoginCode)) {
                        return new ValidationResult("用户名或账号不能为空");
                    }
                    if (string.IsNullOrEmpty(loginRequest.PassWord)) {
                        return new ValidationResult("密码不能为空");
                    }
                }
                /*else if (method == LoginMethod.Phone) {
                    if (string.IsNullOrEmpty(loginRequest.Phone)) {
                        return new ValidationResult("手机号不能为空");
                    }
                    if (string.IsNullOrEmpty(loginRequest.VerificationCode)) {
                        return new ValidationResult("短信验证码不能为空");
                    }
                }*/
                else {
                    return new ValidationResult("第三方平台登录方式未接入");
                }
            }

            return ValidationResult.Success; // 如果没有错误，返回成功
        }
    }
}