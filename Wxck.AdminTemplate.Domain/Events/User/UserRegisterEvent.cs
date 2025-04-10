using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wxck.AdminTemplate.Domain.Events.User {

    public class UserRegisterEvent : EventArgs {

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户代码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegisterTime { get; set; }

        /// <summary>
        /// 注册 IP
        /// </summary>
        public string RegisterIp { get; set; }

        public UserRegisterEvent(string userName, string userCode, DateTime registerTime, string registerIp) {
            UserName = userName;
            UserCode = userCode;
            RegisterTime = registerTime;
            RegisterIp = registerIp;
        }

        public override string ToString() {
            return $"[{RegisterTime}] {UserName}({UserCode}) 注册IP: {RegisterIp}";
        }
    }
}