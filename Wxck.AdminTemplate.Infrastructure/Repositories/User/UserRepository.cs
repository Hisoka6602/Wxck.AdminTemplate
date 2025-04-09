using NLog;
using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Wxck.AdminTemplate.Domain.Attributes;
using Wxck.AdminTemplate.Domain.Entities.User;
using Wxck.AdminTemplate.Domain.Repositories.User;
using System.ComponentModel.DataAnnotations.Schema;
using Wxck.AdminTemplate.Infrastructure.EntityConfigurations;

namespace Wxck.AdminTemplate.Infrastructure.Repositories.User {

    [InjectableRepository(typeof(IUserRepository))]
    public class UserRepository : MemoryCacheRepositoryBase<UserInfoModel, SqlServerContext>, IUserRepository {

        public UserRepository(IDbContextFactory<SqlServerContext> contextFactory, IMemoryCache cache) : base(contextFactory, cache) {
        }

        public new async Task<List<UserInfoModel>?> MemoryCacheData() {
            try {
                var name = typeof(UserInfoModel).GetCustomAttribute<TableAttribute>()?.Name;
                return await _cache.GetOrCreateAsync(name, async entry => {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                    await using var concardContext = await _contextFactory.CreateDbContextAsync();
                    var dbSet = concardContext?.Set<UserInfoModel>();
                    if (dbSet is null || concardContext is null) return null;
                    return await dbSet.AsNoTracking()
                        .Include(b => b.UserVipInfo)
                        .ToListAsync();
                });
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            return null;
        }

        public async Task<KeyValuePair<bool, object>> DetailsInfo(string userCode, CancellationToken token) {
            try {
                await using var concardContext = await _contextFactory.CreateDbContextAsync(token);
                var dbSet = concardContext?.Set<UserInfoModel>();
                if (dbSet is null || concardContext is null) return new KeyValuePair<bool, object>(false, "查询失败");
                var userInfo = await dbSet.AsNoTracking()
                    .Where(w => w.UserCode.Equals(userCode))
                    .Include(b => b.UserVipInfo)
                    .FirstOrDefaultAsync(cancellationToken: token);
                return new KeyValuePair<bool, object>(true, userInfo ?? new UserInfoModel());
            }
            catch (Exception e) {
                NLog.LogManager.GetCurrentClassLogger().Error(e, $"查询失败");
                return new KeyValuePair<bool, object>(false, "查询失败");
            }
        }
    }
}