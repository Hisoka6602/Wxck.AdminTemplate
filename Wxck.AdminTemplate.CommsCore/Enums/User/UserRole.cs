using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wxck.AdminTemplate.CommsCore.Enums.User {

    [Flags]
    public enum UserRole {

        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,

        /// <summary>
        /// 超级管理员
        /// </summary>
        [Description("超级管理员")]
        SuperAdmin = 1 << 0,

        /// <summary>
        /// 运营管理员
        /// </summary>
        [Description("运营管理员")]
        OperatorAdmin = 1 << 1,

        /// <summary>
        /// 普通客户
        /// </summary>
        [Description("普通客户")]
        Customer = 1 << 2,

        /// <summary>
        /// 游客
        /// </summary>
        [Description("游客")]
        Guest = 1 << 3,

        /// <summary>
        /// 客服
        /// </summary>
        [Description("客服")]
        CustomerService = 1 << 7
    }
}