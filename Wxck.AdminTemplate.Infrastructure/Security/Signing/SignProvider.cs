using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using Wxck.AdminTemplate.CrossCutting.Security.Signing;

namespace Wxck.AdminTemplate.Infrastructure.Security.Signing {

    public class SignProvider : ISignProvider {

        public bool IsValid(string md5Content, string secret, string content, string constkey) {
            if (md5Content.Length < 16) return false;
            string sign;
            using (var md5 = MD5.Create()) {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(secret + content + constkey));
                var strResult = BitConverter.ToString(result);
                sign = strResult.Replace("-", "");
            }

            return sign.ToUpper().Equals(md5Content.ToUpper());
        }

        public bool IsValid(string md5Content, string secret, string content) {
            return IsValid(md5Content, secret, content, "Hisoka");
        }

        public bool IsValid(string md5Content, string secret, DateTime validTime, string content) {
            var isValid = IsValid(md5Content, secret, $"{content}{validTime:yyyy-MM-dd HH:mm:00}", "Hisoka");
            if (!isValid) {
                validTime = validTime.AddMinutes(1);
                isValid = IsValid(md5Content, secret, $"{content}{validTime:yyyy-MM-dd HH:mm:00}", "Hisoka");
            }
            return isValid;
        }
    }
}