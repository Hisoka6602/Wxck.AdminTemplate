using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wxck.AdminTemplate.CommsCore.Enums.User {

    public enum UserStatus {

        /// <summary>
        /// 激活
        /// </summary>
        [Description("激活")]
        Active = 0,

        /// <summary>
        /// 冻结
        /// </summary>
        [Description("冻结")]
        Frozen = 1,

        /// <summary>
        /// 失效
        /// </summary>
        [Description("失效")]
        Invalid = 2
    }
}