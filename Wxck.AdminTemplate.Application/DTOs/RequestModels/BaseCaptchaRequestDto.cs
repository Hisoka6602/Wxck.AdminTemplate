using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wxck.AdminTemplate.Application.Validators.Attributes;

namespace Wxck.AdminTemplate.Application.DTOs.RequestModels {

    public class BaseCaptchaRequestDto {

        /// <summary>
        /// 图形验证码Code
        /// </summary>
        [Required(ErrorMessage = "图形验证码Code不能为空!"),
         VerificationCaptcha(ErrorMessage = "图形验证码错误!")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 图形验证码Id
        /// </summary>
        [Required(ErrorMessage = "图形验证码CodeId不能为空!")]
        public string CodeId { get; set; } = string.Empty;
    }
}