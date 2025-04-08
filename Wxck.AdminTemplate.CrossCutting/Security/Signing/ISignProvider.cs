using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wxck.AdminTemplate.CrossCutting.Security.Signing {

    public interface ISignProvider {

        /// <summary>
        /// Md5Sign验证
        /// </summary>
        /// <param name="md5Content"></param>
        /// <param name="secret"></param>
        /// <param name="content"></param>
        /// <param name="constKey"></param>
        /// <returns></returns>
        bool IsValid(string md5Content, string secret, string content, string constKey);

        /// <summary>
        /// Md5Sign验证
        /// </summary>
        /// <param name="md5Content"></param>
        /// <param name="secret"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        bool IsValid(string md5Content, string secret, string content);

        /// <summary>
        /// Md5Sign验证
        /// </summary>
        /// <param name="md5Content"></param>
        /// <param name="secret"></param>
        /// <param name="validTime"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        bool IsValid(string md5Content, string secret, DateTime validTime, string content);
    }
}