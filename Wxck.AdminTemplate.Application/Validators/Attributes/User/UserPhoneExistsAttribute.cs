using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Wxck.AdminTemplate.Domain.Repositories.User;

namespace Wxck.AdminTemplate.Application.Validators.Attributes.User {

    public class UserPhoneExistsAttribute : ValidationAttribute {
        public bool IsExists { get; set; }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
            if (value is not null) {
                var userRepository = validationContext.GetService<IUserRepository>();
                if (userRepository is not null) {
                    var licenseUserInfos = userRepository?.MemoryCacheData().ConfigureAwait(false).GetAwaiter().GetResult();
                    var licenseUserInfo = licenseUserInfos?.FirstOrDefault(f => f.Phone.Equals(value.ToString()));

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