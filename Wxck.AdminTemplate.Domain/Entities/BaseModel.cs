using Wxck.AdminTemplate.Domain.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wxck.AdminTemplate.Domain.Entities {

    public class BaseModel : IEntity<long> {

        [Column("Id"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改时间
        /// </summary>
        [Required, Column("ModifyTime"), InsertOrUpdata]
        public DateTime ModifyTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改IP
        /// </summary>
        [Required, Column("ModifyIp"), InsertOrUpdata]
        public string ModifyIp { get; set; } = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        [Column("Remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
}