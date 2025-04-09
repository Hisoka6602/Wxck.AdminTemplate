using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wxck.AdminTemplate.Domain.Entities.User {

    [Table("User_UserVipInfo", Schema = "dbo")]
    public class UserVipInfoModel : UserBaseForeignKeyInfoModel {

        /// <summary>
        /// Vip名称
        /// </summary>
        [Column("VipName")]
        public string VipName { get; set; } = string.Empty;

        /// <summary>
        /// Vip等级
        /// </summary>
        [Column("VipLevel")]
        public int VipLevel { get; set; }

        /// <summary>
        /// Vip经验
        /// </summary>
        [Column("VipExperience")]
        public int VipExperience { get; set; }

        /// <summary>
        /// Vip积分
        /// </summary>
        [Column("RechargePoints")]
        public decimal RechargePoints { get; set; }

        /// <summary>
        /// Vip折扣（例如：0.9表示九折）
        /// </summary>
        [Column("VipDiscount")]
        public decimal VipDiscount { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
    }
}