﻿using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Wxck.AdminTemplate.Domain.Repositories.User;

namespace Wxck.AdminTemplate.Application.Validators.Attributes.User {

    public class UserCodeExistsAttribute : ValidationAttribute {
        public bool IsExists { get; set; }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
            if (value is not null) {
                var licenseUserRepository = validationContext.GetService<IUserRepository>();
                if (licenseUserRepository is not null) {
                    var licenseUserInfos = licenseUserRepository?.MemoryCacheData().ConfigureAwait(false).GetAwaiter().GetResult();
                    var licenseUserInfo = licenseUserInfos?.FirstOrDefault(f => f.UserCode.Equals(value.ToString()));

                    return (licenseUserInfo != null != IsExists) ? new ValidationResult(this.ErrorMessage) : ValidationResult.Success;
                }
                else {
                    return new ValidationResult("获取配置参数错误!");
                }
            }

            return ValidationResult.Success;
        }
    }
}