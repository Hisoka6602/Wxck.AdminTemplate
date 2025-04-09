using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Wxck.AdminTemplate.Application.Validators.Attributes {

    /// <summary>
    /// 文件验证特性
    /// </summary>
    public class FileMetadataAttribute : ValidationAttribute {
        public string[] AllowedExtensions { get; }
        public long MaxSize { get; } // 以字节为单位

        public FileMetadataAttribute(long maxSize, params string[] allowedExtensions) {
            MaxSize = maxSize;
            AllowedExtensions = allowedExtensions.Select(e => e.ToLowerInvariant()).ToArray();
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
            if (value is not IFormFile file) {
                return new ValidationResult("未发现文件");
            }

            // 获取文件后缀名
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension)) {
                return new ValidationResult($"支持的文件格式: {string.Join(", ", AllowedExtensions)}");
            }

            // 验证文件大小
            if (file.Length > MaxSize) {
                return new ValidationResult($"文件最大上限: ({MaxSize / (1024 * 1024)} MB).");
            }

            return ValidationResult.Success;
        }
    }
}