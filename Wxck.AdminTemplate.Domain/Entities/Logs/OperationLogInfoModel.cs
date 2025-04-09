using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wxck.AdminTemplate.Domain.Entities.Logs {

    /// <summary>
    /// 访问记录
    /// </summary>
    [Table("Log_OperationLog", Schema = "dbo")]
    public class OperationLogInfoModel : BaseInfoModel {

        /// <summary>
        /// 请求内容
        /// </summary>
        [Required, Column("RequestContent")]
        public string RequestContent { get; set; } = string.Empty;

        /// <summary>
        /// 响应内容
        /// </summary>
        [Required, Column("ResponseContent")]
        public string ResponseContent { get; set; } = string.Empty;

        /// <summary>
        /// 操作描述
        /// </summary>
        [Required, Column("OperationDescription")]
        public string OperationDescription { get; set; } = string.Empty;

        /// <summary>
        /// 是否成功
        /// </summary>
        [Required, Column("IsSuccessful")]
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// 请求IP
        /// </summary>
        [Required, Column("RequestIP")]
        public string RequestIp { get; set; } = string.Empty;

        /// <summary>
        /// 操作人
        /// </summary>
        [Required, Column("OperatorId")]
        public long OperatorId { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [Required, Column("OperationTime")]
        public DateTime OperationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 请求方法
        /// </summary>
        [Required, Column("RequestMethod")]
        public string RequestMethod { get; set; } = string.Empty;

        /// <summary>
        /// 访问耗时（单位：毫秒）
        /// </summary>
        [Required, Column("TimeSpent")]
        public long TimeSpent { get; set; }
    }
}