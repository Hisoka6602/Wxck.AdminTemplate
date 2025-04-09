using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wxck.AdminTemplate.Domain.Entities.Logs {

    /// <summary>
    /// 拦截记录
    /// </summary>
    [Table("Log_RiskControlLog", Schema = "dbo")]
    public class RiskControlLogInfo : BaseModel {

        /// <summary>
        /// 被拦截的IP地址
        /// </summary>
        [Column("IpAddress"), Required]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// 拦截时间
        /// </summary>
        [Column("BlockedDate"), Required]
        public DateTime BlockedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 拦截原因
        /// </summary>
        [Column("Reason"), Required]
        public string Reason { get; set; } = string.Empty;
    }
}