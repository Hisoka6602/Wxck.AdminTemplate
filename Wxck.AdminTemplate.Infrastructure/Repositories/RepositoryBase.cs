using NLog;
using System.Reflection;
using System.Transactions;
using System.ComponentModel;
using EFCore.BulkExtensions;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Wxck.AdminTemplate.Domain.Attributes;
using Microsoft.EntityFrameworkCore.Storage;
using Wxck.AdminTemplate.Domain.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wxck.AdminTemplate.Infrastructure.Repositories {

    public class RepositoryBase<T, TContext> : IRepository<T> where T : class
    where TContext : DbContext {
        public readonly IDbContextFactory<TContext> _contextFactory;
        private static SemaphoreSlim _transactionSlim = new(1);

        public RepositoryBase(IDbContextFactory<TContext> contextFactory, IMemoryCache cache) {
            if (contextFactory == null || cache == null) {
                return;
            }
            _contextFactory = contextFactory;
        }

        public async Task<int> ExecuteSqlAsync([NotNull] string sql, CancellationToken token) {
            if (sql.Equals(string.Empty)) return 0;
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                return await concordContext.Database.ExecuteSqlRawAsync(sql, token);
            }
            catch (Win32Exception) {
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
                }
            }
            catch (TaskCanceledException) {
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return 0;
        }

        public async Task<List<T>?> FromSqlRaw([NotNull] string sql, CancellationToken token) {
            if (sql.Equals(string.Empty)) return null;

            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                {
                    var dbSet = concordContext?.Set<T>();
                    if (dbSet is null) return null;
                    return await dbSet.FromSqlRaw(sql).ToListAsync(cancellationToken: token);
                }
            }
            catch (Win32Exception) {
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
                }
            }
            catch (TaskCanceledException) {
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            return null;
        }

        public async Task<bool> Insert([NotNull] T entity, CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        if (contextTransaction is not null) {
                            var dbSet = concordContext?.Set<T>();
                            await dbSet.AddAsync(entity, token);
                            await concordContext?.SaveChangesAsync(token);
                            await contextTransaction.CommitAsync(token);

                            return true;
                        }
                    }

                    return false;
                });
            }
            catch (Win32Exception) {
                await contextTransaction?.RollbackAsync(token)!;
            }
            catch (TaskCanceledException) {
                await contextTransaction?.RollbackAsync(token)!;
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            return false;
        }

        public async void InsertAsync([NotNull] T entity, CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        if (contextTransaction is not null) {
                            var dbSet = concordContext?.Set<T>();
                            await dbSet.AddAsync(entity, token);
                            await concordContext?.SaveChangesAsync(token);
                            await contextTransaction.CommitAsync(token);
                        }
                    }
                });
            }
            catch (Win32Exception) {
                await contextTransaction?.RollbackAsync(token)!;
            }
            catch (TaskCanceledException) {
                await contextTransaction?.RollbackAsync(token)!;
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
        }

        public async Task<bool> InsertRange([NotNull] List<T> entitylist, CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        if (contextTransaction is not null) {
                            await concordContext.BulkInsertAsync(entitylist, new BulkConfig() {
                                UseTempDB = true,
                                UniqueTableNameTempDb = false,
                                PreserveInsertOrder = true,
                                SetOutputIdentity = false
                            }, type: typeof(T), cancellationToken: token);
                            await contextTransaction.CommitAsync(token);
                            return true;
                        }

                        return false;
                    }
                });
            }
            catch (Win32Exception) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (TaskCanceledException) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, $"[{typeof(T).Name}]{e}");
                }
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (Exception e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, $"[{typeof(T).Name}]{e}");
            }

            return false;
        }

        public async Task<bool> InsertOrUpdate([NotNull] T entity, CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                var propertyInfos = typeof(T).GetProperties(
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        await concordContext.BulkInsertOrUpdateAsync(new List<T>() {
                            entity
                        }, new BulkConfig() {
                            UseTempDB = true,
                            UniqueTableNameTempDb = false,
                            PropertiesToIncludeOnUpdate = propertyInfos?.Where(
                                w => w.GetCustomAttribute<InsertOrUpdataAttribute>() != null)?.Select(s => s.Name)?.ToList(),
                            PropertiesToExclude = propertyInfos
                                ?.Where(w => w.GetCustomAttribute<DatabaseGeneratedAttribute>() != null ||
                                             w.GetCustomAttribute<ExcludeOnUpdateAttribute>() != null)
                                ?.Select(s => s.Name)?.ToList(),
                            PreserveInsertOrder = true,
                            SetOutputIdentity = false,
                            UpdateByProperties = propertyInfos
                                ?.Where(w => w.GetCustomAttribute<UpdateByAttribute>() != null)
                                ?.Select(s => s.Name)?.ToList(),
                        }, type: typeof(T), cancellationToken: token);
                        await contextTransaction.CommitAsync(token);
                        return true;
                    }
                });
            }
            catch (TransactionException e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            catch (TaskCanceledException) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
                }
            }
            catch (Exception e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return false;
        }

        public async Task<bool> InsertOrUpdateRange([NotNull] List<T> entities,
            CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                await _transactionSlim.WaitAsync(token);
                var propertyInfos = typeof(T).GetProperties(
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        if (contextTransaction is not null) {
                            await concordContext.BulkInsertOrUpdateAsync(entities, new BulkConfig() {
                                UseTempDB = true,
                                UniqueTableNameTempDb = false,
                                PropertiesToIncludeOnUpdate = propertyInfos?.Where(
                                    w => w.GetCustomAttribute<InsertOrUpdataAttribute>() != null
                                )?.Select(s => s.Name)?.ToList(),
                                /*PropertiesToExcludeOnUpdate = propertyInfos
                                        ?.Where(w => w.GetCustomAttribute<DatabaseGeneratedAttribute>() != null ||
                                                     w.GetCustomAttribute<ExcludeOnUpdateAttribute>() != null)
                                        ?.Select(s => s.Name)?.ToList(),*/
                                /*PropertiesToInclude = propertyInfos?.Where(
                                        w => w.GetCustomAttribute<InsertOrUpdataAttribute>() != null)?.Select(s => s.Name)?.ToList(),*/

                                /*PropertiesToExclude = propertyInfos
                                        ?.Where(w => w.GetCustomAttribute<InsertOrUpdataAttribute>() == null)
                                        ?.Select(s => s.Name)?.ToList(),*/
                                PropertiesToExclude = propertyInfos
                                    ?.Where(w => w.GetCustomAttribute<DatabaseGeneratedAttribute>() != null)
                                    ?.Select(s => s.Name)?.ToList(),
                                UpdateByProperties = propertyInfos
                                    ?.Where(w => w.GetCustomAttribute<UpdateByAttribute>() != null)
                                    ?.Select(s => s.Name)?.ToList(),
                                PreserveInsertOrder = true,
                                SetOutputIdentity = false
                            }, type: typeof(T), cancellationToken: token);
                            await contextTransaction.CommitAsync(token);
                            return true;
                        }

                        return false;
                    }
                });
            }
            catch (TransactionException e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            catch (TaskCanceledException) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
                }
            }
            catch (Exception e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            finally {
                _transactionSlim.Release();
            }
            return false;
        }

        public async Task<bool> InsertOrUpdate([NotNull] T entity, [NotNull] Expression<Func<T, object>> excludeColumns, CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                var exclude = new List<string>();
                var memberInfos = ((NewExpression)excludeColumns.Body).Members;
                if (memberInfos is not null) {
                    exclude = memberInfos.Select(p => p.Name).ToList();
                }

                var propertyInfos = typeof(T).GetProperties(
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        await concordContext.BulkInsertOrUpdateAsync(new List<T>() {
                            entity
                        }, new BulkConfig() {
                            UseTempDB = true,
                            UniqueTableNameTempDb = false,
                            PropertiesToExcludeOnUpdate = exclude,
                            PreserveInsertOrder = true,
                            SetOutputIdentity = false,
                            UpdateByProperties = propertyInfos
                                ?.Where(w => w.GetCustomAttribute<UpdateByAttribute>() != null)
                                ?.Select(s => s.Name)?.ToList(),
                        }, type: typeof(T), cancellationToken: token);
                        await contextTransaction.CommitAsync(token);
                        return true;
                    }
                });
            }
            catch (TransactionException e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            catch (TaskCanceledException) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
                }
            }
            catch (Exception e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return false;
        }

        public async Task<bool> InsertOrUpdateRange([NotNull] List<T> entities, [NotNull] Expression<Func<T, object>> excludeColumns, CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                var exclude = new List<string>();
                var memberInfos = ((NewExpression)excludeColumns.Body).Members;
                if (memberInfos is not null) {
                    exclude = memberInfos.Select(p => p.Name).ToList();
                }
                await _transactionSlim.WaitAsync(token);
                var propertyInfos = typeof(T).GetProperties(
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        if (contextTransaction is null) return false;
                        await concordContext.BulkInsertOrUpdateAsync(entities, new BulkConfig() {
                            UseTempDB = true,
                            UniqueTableNameTempDb = false,
                            PropertiesToExcludeOnUpdate = exclude,
                            UpdateByProperties = propertyInfos
                                ?.Where(w => w.GetCustomAttribute<UpdateByAttribute>() != null)
                                ?.Select(s => s.Name)?.ToList(),
                            PreserveInsertOrder = true,
                            SetOutputIdentity = false
                        }, type: typeof(T), cancellationToken: token);
                        await contextTransaction.CommitAsync(token);
                        return true;
                    }
                });
            }
            catch (TransactionException e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            catch (TaskCanceledException) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
                }
            }
            catch (Exception e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            finally {
                _transactionSlim.Release();
            }
            return false;
        }

        public async Task<bool> Update([NotNull] T entity, CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        if (contextTransaction is not null) {
                            var dbSet = concordContext?.Set<T>();
                            dbSet.Update(entity);
                            await concordContext?.SaveChangesAsync(token)!;
                            await contextTransaction.CommitAsync(token);

                            return true;
                        }
                    }

                    return false;
                });
            }
            catch (Win32Exception) {
                await contextTransaction?.RollbackAsync(token)!;
            }
            catch (TaskCanceledException) {
                await contextTransaction?.RollbackAsync(token)!;
            }
            catch (Exception e) {
                await contextTransaction?.RollbackAsync(token)!;
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            return false;
        }

        public async Task<bool> UpdateRange(List<T> entities, CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        if (contextTransaction is not null) {
                            await concordContext.BulkUpdateAsync(entities, new BulkConfig() {
                                UseTempDB = true
                            }, type: typeof(T), cancellationToken: token);
                            await contextTransaction.CommitAsync(token);
                            return true;
                        }
                        return false;
                    }
                });
            }
            catch (Win32Exception) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (TaskCanceledException) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, $"[{typeof(T).Name}]{e}");
                }
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (Exception e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, $"[{typeof(T).Name}]{e}");
            }

            return false;
        }

        public async Task<bool> UpdateRange([NotNull] List<T> entities, [NotNull] Expression<Func<T, object>> excludeColumns, CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                var exclude = new List<string>();
                var memberInfos = ((NewExpression)excludeColumns.Body).Members;
                if (memberInfos is not null) {
                    exclude = memberInfos.Select(p => p.Name).ToList();
                }

                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        if (contextTransaction is null) return false;
                        await concordContext.BulkUpdateAsync(entities, new BulkConfig() {
                            UseTempDB = true,
                            PropertiesToExcludeOnUpdate = exclude,
                        }, type: typeof(T), cancellationToken: token);
                        await contextTransaction.CommitAsync(token);
                        return true;
                    }
                });
            }
            catch (Win32Exception) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (TaskCanceledException) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, $"[{typeof(T).Name}]{e}");
                }
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (Exception e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, $"[{typeof(T).Name}]{e}");
            }

            return false;
        }

        public async Task<bool> Delete([NotNull] T entity, CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        if (contextTransaction is not null) {
                            var dbSet = concordContext?.Set<T>();
                            dbSet?.Remove(entity);
                            await concordContext?.SaveChangesAsync(token)!;
                            await contextTransaction.CommitAsync(token);
                            return true;
                        }
                    }

                    return false;
                });
            }
            catch (Win32Exception) {
                await contextTransaction?.RollbackAsync(token)!;
            }
            catch (TaskCanceledException) {
                await contextTransaction?.RollbackAsync(token)!;
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            return false;
        }

        public async Task<bool> DeleteRange([NotNull] List<T> entities, CancellationToken token) {
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var dbSet = concordContext?.Set<T>();
                if (dbSet is not null && concordContext is not null) {
                    await concordContext.BulkDeleteAsync(entities, cancellationToken: token);
                    return true;
                }
            }
            catch (Win32Exception) {
            }
            catch (TaskCanceledException) {
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
                }
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return false;
        }

        [Obsolete("Obsolete")]
        public async Task<int> DeleteCount(int count, CancellationToken token) {
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                {
                    var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                    var dbSet = concordContext?.Set<T>();
                    if (dbSet is null) return 0;

                    return await dbSet.Take(count).BatchDeleteAsync(cancellationToken: token);
                }
            }
            catch (Win32Exception) {
            }
            catch (TaskCanceledException) {
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
                }
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return 0;
        }

        public async Task<int> DeleteCount(int count, Expression<Func<T, bool>> @where, CancellationToken token) {
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                {
                    var name = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
                    var dbSet = concordContext?.Set<T>();
                    if (dbSet is null) return 0;

                    return await dbSet.Where(where).Take(count).BatchDeleteAsync(cancellationToken: token);
                }
            }
            catch (Win32Exception) {
            }
            catch (TaskCanceledException) {
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
                }
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return 0;
        }

        public async Task<List<T>?> Select([NotNull] Expression<Func<T, bool>> @where, int pageIndex, int pageSize, CancellationToken token) {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > 1000 ? 1000 : pageSize;
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var dbSet = concordContext?.Set<T>();
                if (dbSet is null) return null;
                return await dbSet.AsNoTracking()?.Where(where)
                    ?.Skip(pageIndex * pageSize)?.Take(pageSize)?.ToListAsync(token)!;
            }
            catch (Win32Exception) {
            }
            catch (TaskCanceledException) {
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return null;
        }

        public async Task<List<T>?> Select<TOrder>([NotNull] Expression<Func<T, bool>> @where, Expression<Func<T, TOrder>> order, int pageIndex, int pageSize, CancellationToken token) {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > 1000 ? 1000 : pageSize;
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var dbSet = concordContext?.Set<T>();
                if (dbSet is null) return null;
                return await dbSet.AsNoTracking()?.Where(where)?.OrderBy(order)
                    ?.Skip(pageIndex * pageSize)?.Take(pageSize)?.ToListAsync(token)!;
            }
            catch (Win32Exception) {
            }
            catch (TaskCanceledException) {
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return null;
        }

        public async Task<List<T>?> Select<TOrder>([NotNull] Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order, CancellationToken token) {
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var dbSet = concordContext?.Set<T>();
                if (dbSet is null) return null;
                return await dbSet.AsNoTracking()?.Where(where)?.OrderBy(order)
                    ?.ToListAsync(token)!;
            }
            catch (Win32Exception) {
            }
            catch (TaskCanceledException) {
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }
            return null;
        }

        public async Task<List<T>?> SelectOrderByDescending<TOrder>([NotNull] Expression<Func<T, bool>> @where, Expression<Func<T, TOrder>> order, int pageIndex, int pageSize,
            CancellationToken token) {
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            //pageSize = pageSize > 1000 ? 1000 : pageSize;
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var dbSet = concordContext?.Set<T>();
                if (dbSet is null) return null;
                return await dbSet.AsNoTracking()?.Where(where)?.OrderByDescending(order)
                    ?.Skip(pageIndex * pageSize)?.Take(pageSize)?.ToListAsync(token)!;
            }
            catch (Win32Exception) {
            }
            catch (TaskCanceledException) {
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return null;
        }

        public async Task<List<T>?> SelectOrderByDescending<TOrder>([NotNull] Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order, CancellationToken token) {
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var dbSet = concordContext?.Set<T>();
                if (dbSet is null) return null;
                return await dbSet.AsNoTracking()?.Where(where)?.OrderByDescending(order)
                    .ToListAsync(token)!;
            }
            catch (Win32Exception) {
            }
            catch (TaskCanceledException) {
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return null;
        }

        public async Task<T?> FirstOrDefault([NotNull] Expression<Func<T, bool>> @where, CancellationToken token) {
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var dbSet = concordContext?.Set<T>();
                if (dbSet is null) return null;
                return await dbSet.AsNoTracking().Where(where).FirstOrDefaultAsync(token);
            }
            catch (Win32Exception) {
            }
            catch (TaskCanceledException) {
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return null;
        }

        public async Task<int> Total([NotNull] Expression<Func<T, bool>> @where, CancellationToken token) {
            try {
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var dbSet = concordContext?.Set<T>();
                if (dbSet is null) return 0;
                return await dbSet.AsNoTracking().Where(where).CountAsync(token);
            }
            catch (Win32Exception) {
            }
            catch (TaskCanceledException) {
            }
            catch (Exception e) {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, e.ToString());
            }

            return 0;
        }

        public async Task<bool> SyncEntities([NotNull] List<T> entities, CancellationToken token) {
            IDbContextTransaction? contextTransaction = null;
            try {
                var propertyInfos = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance |
                                                            System.Reflection.BindingFlags.Public);
                var excludeOnUpdateColumns = propertyInfos.Where(w => w.GetCustomAttribute<ExcludeOnUpdateAttribute>() != null ||
                                                                      w.GetCustomAttribute<DatabaseGeneratedAttribute>() != null)
                    .Select(s => s.Name)?.ToList();
                await using var concordContext = await _contextFactory.CreateDbContextAsync(token);
                var strategy = concordContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () => {
                    await using (contextTransaction = await concordContext.Database.BeginTransactionAsync(token)) {
                        await concordContext.BulkInsertOrUpdateOrDeleteAsync(entities, new BulkConfig() {
                            UseTempDB = true,
                            PropertiesToExcludeOnUpdate = excludeOnUpdateColumns
                        }, type: typeof(T), cancellationToken: token);
                        await contextTransaction.CommitAsync(token);
                        return true;
                    }
                });
            }
            catch (Win32Exception) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (TaskCanceledException) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (SqlException e) {
                if (e.Number != -2) {
                    LogManager.GetCurrentClassLogger().Log(LogLevel.Error, $"[{typeof(T).Name}]{e}");
                }
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
            }
            catch (Exception e) {
                contextTransaction?.RollbackAsync(token).ConfigureAwait(false);
                LogManager.GetCurrentClassLogger().Log(LogLevel.Error, $"[{typeof(T).Name}]{e}");
            }

            return false;
        }
    }
}