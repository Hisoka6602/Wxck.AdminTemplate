using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Wxck.AdminTemplate.Domain.Entities.Logs;

namespace Wxck.AdminTemplate.Domain.Repositories.Logs {

    public interface IOperationLogRepository : IRepository<OperationLogInfoModel> {

        /// <summary>
        /// 删除指定时间之前的操作日志
        /// </summary>
        /// <param name="beforeDate">指定的日期时间，删除该时间之前的日志</param>
        /// <param name="token">取消标记</param>
        /// <returns>操作是否成功</returns>
        Task<KeyValuePair<bool, string>> DeleteLogsBeforeDate(DateTime beforeDate, CancellationToken token = default);

        /// <summary>
        /// 删除指定条数之前的操作日志
        /// </summary>
        /// <param name="count">指定的条数，删除最旧的记录</param>
        /// <param name="token">取消标记</param>
        /// <returns>操作是否成功</returns>
        Task<KeyValuePair<bool, string>> DeleteLogsBeforeCount(int count, CancellationToken token = default);

        /// <summary>
        /// 清空所有操作日志
        /// </summary>
        /// <param name="token">取消标记</param>
        /// <returns>操作是否成功</returns>
        Task<KeyValuePair<bool, string>> ClearLogs(CancellationToken token = default);
    }
}