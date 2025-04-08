using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wxck.AdminTemplate.CrossCutting.EventBus.Events {

    public class MemoryTableChangedEvent {

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; init; } = string.Empty;

        /// <summary>
        /// 变更操作名称
        /// </summary>
        public string Operation { get; init; } = string.Empty;

        /// <summary>
        /// 变更发生的时间
        /// </summary>
        public DateTime ChangedAt { get; init; }
    }
}