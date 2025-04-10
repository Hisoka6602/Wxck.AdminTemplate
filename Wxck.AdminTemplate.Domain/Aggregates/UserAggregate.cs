using Wxck.AdminTemplate.Domain.Events.User;
using Wxck.AdminTemplate.CommsCore.Enums.User;
using Wxck.AdminTemplate.Domain.Entities.User;
using Wxck.AdminTemplate.CrossCutting.EventBus;

namespace Wxck.AdminTemplate.Domain.Aggregates {

    public class UserAggregate : AggregateRoot<List<UserInfoModel>> {

        public UserAggregate(List<UserInfoModel> entity) : base(entity) {
            UserInfos = entity;
        }

        public List<UserInfoModel> UserInfos { get; set; }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="ipAddress"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static UserInfoModel Register(string userCode, string userName, string password, string ipAddress, string phone = "") {
            var userInfo = new UserInfoModel {
                UserCode = userCode,
                UserName = userName,
                Phone = phone,
                PassWord = password,
                Status = UserStatus.Active,
                Role = UserRole.Customer,
                ModifyIp = ipAddress
            };
            EventAggregator.Instance.Publish(new UserRegisterEvent(userName, userCode, DateTime.Now, ipAddress));
            // 在这里你可以触发一些领域事件，例如 UserRegisteredEvent
            return userInfo;
        }

        /// <summary>
        /// 登录用户
        /// </summary>
        /// <param name="loginCode"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserInfoModel? LoginForLoginCode(string loginCode, string password) {
            // 简单的登录逻辑，通常要通过加密后的密码进行比较
            //触发领域事件
            // 密码应通过加密验证
            var model = UserInfos.FirstOrDefault(f => (f.UserCode.Equals(loginCode) || f.UserName.Equals(loginCode)) &&
                                                               f.PassWord.Equals(password));

            //发布登录事件
            return model;
        }

        public UserInfoModel? LoginForPhone(string phone) {
            // 简单的登录逻辑，通常要通过加密后的密码进行比较
            //触发领域事件
            var model = UserInfos.FirstOrDefault(f => f.Phone.Equals(phone));

            //发布登录事件
            return model;
        }

        public UserInfoModel? LoginForPlatformToken(string phone) {
            // 简单的登录逻辑，通常要通过加密后的密码进行比较
            //触发领域事件
            return null;
        }

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <returns></returns>
        public UserInfoModel? GetUserInfo(string userCode) {
            //触发领域事件
            return UserInfos.FirstOrDefault(f => f.UserCode.Equals(userCode));
        }
    }
}