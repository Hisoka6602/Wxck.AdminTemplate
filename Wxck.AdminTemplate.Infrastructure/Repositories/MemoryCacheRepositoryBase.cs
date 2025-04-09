using NLog;
using System.Reflection;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Wxck.AdminTemplate.Domain.Repositories;
using Wxck.AdminTemplate.CrossCutting.EventBus;
using System.ComponentModel.DataAnnotations.Schema;
using Wxck.AdminTemplate.CrossCutting.EventBus.Events;

namespace Wxck.AdminTemplate.Infrastructure.Repositories {

    public class MemoryCacheRepositoryBase<T, TContext> : RepositoryBase<T, TContext>, IMemoryCacheRepository<T> where T : class
    where TContext : DbContext {
        private new readonly IDbContextFactory<TContext> _contextFactory;
        public readonly IMemoryCache _cache;

        public MemoryCacheRepositoryBase(IDbContextFactory<TContext> contextFactory, IMemoryCache cache) : base(contextFactory, cache) {
            _contextFactory = contextFactory;
            _cache = cache;
        }

        public async Task<List<T>?> MemoryCacheData() {
            try {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                return await _cache.GetOrCreateAsync(name, async entry => {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                    await using var concardContext = await _contextFactory.CreateDbContextAsync();
                    var dbSet = concardContext?.Set<T>();
                    if (dbSet is null || concardContext is null) return null;
                    var query = dbSet.AsQueryable();
                    var navigationProperties = concardContext.Model.FindEntityType(typeof(T))
                        ?.GetNavigations()
                        .Select(n => n.Name);
                    if (navigationProperties != null)
                        query = navigationProperties.Aggregate(query,
                            (current, navProp) => current.Include(navProp));

                    return await query.AsNoTracking().ToListAsync();
                });
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            return null;
        }

        public async Task<bool> ClearCache() {
            var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
            try {
                _cache.Remove(name ?? string.Empty);

                return await Task.FromResult(true);
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return await Task.FromResult(false);
        }

        public new async Task<bool> Insert([NotNull] T entity, CancellationToken token) {
            var insert = await base.Insert(entity, token);
            if (insert) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);

                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }
            return insert;
        }

        public new void InsertAsync([NotNull] T entity, CancellationToken token) {
            Task.Run(async () => {
                var insert = await base.Insert(entity, token);
                if (insert) {
                    var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                    _cache.Remove(name ?? string.Empty);

                    EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                        ChangedAt = DateTime.Now,
                        TableName = name ?? string.Empty,
                        Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                    });
                }

                return insert;
            }, token);
        }

        public new async Task<bool> InsertRange([NotNull] List<T> entities, CancellationToken token = default) {
            var insertRange = await base.InsertRange(entities, token);
            if (insertRange) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }

            return insertRange;
        }

        public new async Task<bool> Update([NotNull] T entity, CancellationToken token) {
            var update = await base.Update(entity, token);
            if (update) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }
            return update;
        }

        public new async Task<bool> InsertOrUpdate([NotNull] T entity, CancellationToken token) {
            var insertOrUpdate = await base.InsertOrUpdate(entity, token);
            if (insertOrUpdate) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }
            return insertOrUpdate;
        }

        public new async Task<bool> InsertOrUpdate([NotNull] T entity,
            [NotNull] Expression<Func<T, object>> excludeColumns, CancellationToken token = default) {
            var insertOrUpdate = await base.InsertOrUpdate(entity, excludeColumns, token);
            if (insertOrUpdate) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }
            return insertOrUpdate;
        }

        public new async Task<bool> InsertOrUpdateRange([NotNull] List<T> entities,
            CancellationToken token = default) {
            var insertOrUpdateRange = await base.InsertOrUpdateRange(entities, token);
            if (insertOrUpdateRange) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }
            return insertOrUpdateRange;
        }

        public new async Task<bool> InsertOrUpdateRange([NotNull] List<T> entities,
            [NotNull] Expression<Func<T, object>> excludeColumns,
            CancellationToken token = default) {
            var insertOrUpdateRange = await base.InsertOrUpdateRange(entities, excludeColumns, token);
            if (insertOrUpdateRange) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }
            return insertOrUpdateRange;
        }

        public new async Task<bool> UpdateRange(List<T> entities, CancellationToken token = default) {
            var updateRange = await base.UpdateRange(entities, token);
            if (updateRange) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }
            return updateRange;
        }

        public new async Task<bool> UpdateRange([NotNull] List<T> entities,
            [NotNull] Expression<Func<T, object>> excludeColumns,
            CancellationToken token = default) {
            var updateRange = await base.UpdateRange(entities, excludeColumns, token);
            if (updateRange) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }
            return updateRange;
        }

        public new async Task<bool> Delete([NotNull] T entity, CancellationToken token) {
            var delete = await base.Delete(entity, token);
            if (delete) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }
            return delete;
        }

        public new async Task<bool> DeleteRange([NotNull] List<T> entities, CancellationToken token) {
            var delete = await base.DeleteRange(entities, token);
            if (delete) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }
            return delete;
        }

        public new async Task<int> DeleteCount(int count, CancellationToken token) {
            var deleteCount = await base.DeleteCount(count, token);
            if (deleteCount > 0) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }

            return deleteCount;
        }

        public new async Task<int> DeleteCount(int count, Expression<Func<T, bool>> @where, CancellationToken token) {
            var deleteCount = await base.DeleteCount(count, @where, token);
            if (deleteCount > 0) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }

            return deleteCount;
        }

        public new async Task<bool> SyncEntities([NotNull] List<T> entities, CancellationToken token) {
            var syncEntities = await base.SyncEntities(entities, token);
            if (syncEntities) {
                var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                _cache.Remove(name ?? string.Empty);
                _cache.Remove(name ?? string.Empty);
                EventAggregator.Instance.Publish(new MemoryTableChangedEvent {
                    ChangedAt = DateTime.Now,
                    TableName = name ?? string.Empty,
                    Operation = MethodBase.GetCurrentMethod()?.Name ?? "Unknown"
                });
            }
            return syncEntities;
        }
    }
}