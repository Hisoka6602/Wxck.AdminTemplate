using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Wxck.AdminTemplate.Domain.Attributes;
using Wxck.AdminTemplate.Domain.Entities.Logs;
using Wxck.AdminTemplate.Domain.Repositories.Logs;
using Wxck.AdminTemplate.Domain.Repositories.User;
using Wxck.AdminTemplate.Infrastructure.EntityConfigurations;

namespace Wxck.AdminTemplate.Infrastructure.Repositories.Logs {

    [InjectableRepository(typeof(IOperationLogRepository))]
    public class OperationLogRepository : RepositoryBase<OperationLogInfoModel, SqlServerContext>, IOperationLogRepository {

        public OperationLogRepository(IDbContextFactory<SqlServerContext> contextFactory, IMemoryCache cache) : base(contextFactory, cache) {
        }

        public async Task<KeyValuePair<bool, string>> DeleteLogsBeforeDate(DateTime beforeDate, CancellationToken token = default) {
            try {
                await using var context = await _contextFactory.CreateDbContextAsync(token);
                var logsToDelete = context.Set<OperationLogInfoModel>().AsNoTracking().Where(log => log.OperationTime < beforeDate);
                context.Set<OperationLogInfoModel>().RemoveRange(logsToDelete);
                await context.SaveChangesAsync(token);
                return new KeyValuePair<bool, string>(true, "删除成功");
            }
            catch (Exception ex) {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "删除日志失败");
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> DeleteLogsBeforeCount(int count, CancellationToken token = default) {
            try {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var logsToDelete = context.Set<OperationLogInfoModel>().AsNoTracking().OrderBy(log => log.OperationTime).Take(count);
                context.Set<OperationLogInfoModel>().RemoveRange(logsToDelete);
                await context.SaveChangesAsync(token);
                return new KeyValuePair<bool, string>(true, "Logs deleted successfully.");
            }
            catch (Exception ex) {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "删除日志失败");
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> ClearLogs(CancellationToken token = default) {
            try {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var allLogs = context.Set<OperationLogInfoModel>().AsNoTracking();
                context.Set<OperationLogInfoModel>().RemoveRange(allLogs);
                await context.SaveChangesAsync(token);
                return new KeyValuePair<bool, string>(true, "All logs cleared successfully.");
            }
            catch (Exception ex) {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "删除日志失败");
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }
    }
}