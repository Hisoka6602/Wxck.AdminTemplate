using System;
using System.Linq;
using System.Text;
using System.Numerics;
using Lazy.Captcha.Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Wxck.AdminTemplate.Application.DTOs.RequestModels;

namespace Wxck.AdminTemplate.Application.Validators.Attributes {

    public class VerificationCaptchaAttribute : ValidationAttribute {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
            var captcha = validationContext.GetService<ICaptcha>();
            if (value is string verificationCode &&
                captcha is not null) {
                if (validationContext.ObjectInstance is BaseCaptchaRequestDto baseCaptchaRequestDto) {
                    var validate = captcha.Validate(baseCaptchaRequestDto.CodeId, baseCaptchaRequestDto.Code);
                    if (!validate) {
                        return new ValidationResult(ErrorMessage);
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}