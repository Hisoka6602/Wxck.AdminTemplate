using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Wxck.AdminTemplate.CommsCore.Enums.User;

namespace Wxck.AdminTemplate.Application.Services.User {

    public interface IUserService {

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="ipAddress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<KeyValuePair<bool, object>> Register(string userCode,
            string userName, string passWord, string ipAddress, CancellationToken token = default);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginCode"></param>
        /// <param name="passWord"></param>
        /// <param name="phone"></param>
        /// <param name="platformToken"></param>
        /// <param name="loginMethod"></param>
        /// <param name="ipAddress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<KeyValuePair<bool, object>> Login(string loginCode, string passWord, string? phone, string? platformToken, LoginMethod loginMethod,
            string ipAddress,
            CancellationToken token = default);

        /// <summary>
        /// 个人信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<KeyValuePair<bool, object>> Info(string? userCode, CancellationToken token = default);
    }
}