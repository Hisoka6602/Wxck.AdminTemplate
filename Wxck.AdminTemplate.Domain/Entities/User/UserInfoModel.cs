using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Wxck.AdminTemplate.Domain.Attributes;
using System.ComponentModel.DataAnnotations;
using Wxck.AdminTemplate.CommsCore.Enums.User;
using Wxck.AdminTemplate.CrossCutting.Converters;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wxck.AdminTemplate.Domain.Entities.User {

    [Table("User_UserInfo", Schema = "dbo")]
    public class UserInfoModel : BaseInfoModel {

        /// <summary>
        /// 用户代码
        /// </summary>
        [Required, Column("UserCode"), UpdateBy]
        public string UserCode { get; set; } = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        [Required, Column("UserName"), InsertOrUpdata]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Required, Column("PassWord"), InsertOrUpdata, JsonIgnore]
        public string PassWord { get; set; } = string.Empty;

        /// <summary>
        /// 手机号
        /// </summary>
        [Required, Column("Phone"), InsertOrUpdata,
         JsonConverter(typeof(PhoneMaskConverter))]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required, Column("Email"), InsertOrUpdata]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 角色
        /// </summary>
        [Required, Column("Role")]
        public UserRole Role { get; set; } = UserRole.None;

        /// <summary>
        /// 用户状态
        /// </summary>
        [Required, Column("Status")]
        public UserStatus Status { get; set; } = UserStatus.Active;

        /// <summary>
        /// 用户图片
        /// </summary>
        [Column("UserIcon")]
        public string? UserIcon { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        [Column("InviteCode")]
        public string? InviteCode { get; set; }

        /// <summary>
        /// 用户VIP信息
        /// </summary>
        public virtual UserVipInfoModel? UserVipInfo { get; set; }
    }
}