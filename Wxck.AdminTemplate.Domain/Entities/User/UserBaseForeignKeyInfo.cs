using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wxck.AdminTemplate.Domain.Entities.User {

    public class UserBaseForeignKeyInfoModel : BaseInfoModel {

        /// <summary>
        /// Id
        /// </summary>
        [Column("UserId"), JsonIgnore]
        public long UserId { get; set; }

        /// <summary>
        /// 用户信息外键
        /// </summary>
        [ForeignKey("Id"), JsonIgnore]
        public virtual UserInfoModel? UserInfo { get; set; }
    }
}