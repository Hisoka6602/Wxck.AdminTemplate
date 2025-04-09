using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wxck.AdminTemplate.Domain.Entities.User {

    public class UserBaseForeignKeyInfoModel : BaseInfoModel {

        [Column("UserId"), JsonIgnore]
        public long UserId { get; set; }

        [ForeignKey("Id"), JsonIgnore]
        public virtual UserInfoModel? UserInfo { get; set; }
    }
}