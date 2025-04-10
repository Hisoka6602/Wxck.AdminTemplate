using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Wxck.AdminTemplate.Domain.Attributes;
using Wxck.AdminTemplate.Domain.Aggregates;
using Wxck.AdminTemplate.CommsCore.Enums.User;
using Wxck.AdminTemplate.Domain.Entities.User;
using Wxck.AdminTemplate.CrossCutting.Auth.Jwt;
using Wxck.AdminTemplate.Domain.Repositories.User;

namespace Wxck.AdminTemplate.Application.Services.User {

    [InjectableService(typeof(IUserService))]
    public class UserService : IUserService {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticateService _authenticateService;

        public UserService(IUserRepository userRepository,
            IAuthenticateService authenticateService) {
            _userRepository = userRepository;
            _authenticateService = authenticateService;
        }

        public async Task<KeyValuePair<bool, object>> Register(string userCode, string userName,
            string passWord, string ipAddress, CancellationToken token = default) {
            var user = UserAggregate.Register(userCode, userName, passWord, ipAddress);

            var insert = await _userRepository.Insert(user, token);
            return insert ? new KeyValuePair<bool, object>(true, "注册成功") : new KeyValuePair<bool, object>(true, "注册失败");
        }

        public async Task<KeyValuePair<bool, object>> Login(string loginCode, string passWord,
            string? phone, string? platformToken, LoginMethod loginMethod, string ipAddress,
            CancellationToken token = default) {
            var memoryCacheData = await _userRepository.MemoryCacheData();
            var infoModel = loginMethod switch {
                LoginMethod.LoginCode =>
                    new UserAggregate(memoryCacheData ?? []).LoginForLoginCode(loginCode, passWord),
                LoginMethod.Phone => new UserAggregate(memoryCacheData ?? []).LoginForPhone(phone ?? string.Empty),
                _ => new UserAggregate(memoryCacheData ?? []).LoginForPlatformToken(platformToken ?? string.Empty)
            };
            if (infoModel is null) {
                return new KeyValuePair<bool, object>(false, "登录失败");
            }
            //获取token
            var isAuthenticated = _authenticateService.IsAuthenticated(new LoginRequestDto() {
                PassWord = infoModel.PassWord,
                UserCode = infoModel.UserCode
            }, out var logInToken);
            return !isAuthenticated ? new KeyValuePair<bool, object>(false, "生成Token失败!") : new KeyValuePair<bool, object>(true, logInToken);
        }

        public async Task<KeyValuePair<bool, object>> Info(string? userCode, CancellationToken token = default) {
            var memoryCacheData = await _userRepository.MemoryCacheData();
            var userInfoModel = new UserAggregate(memoryCacheData ?? []).GetUserInfo(userCode ?? string.Empty);
            return userInfoModel is null ? new KeyValuePair<bool, object>(false, "查询失败") : new KeyValuePair<bool, object>(true, userInfoModel);
        }
    }
}