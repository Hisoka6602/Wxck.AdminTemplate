using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Wxck.AdminTemplate.Domain.Entities.User;
using Wxck.AdminTemplate.Domain.Entities.Logs;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Wxck.AdminTemplate.Infrastructure.EntityConfigurations {

    public sealed class SqlServerContext : DbContext {

        public SqlServerContext(DbContextOptions<SqlServerContext> options) : base(options) {
            try {
                var databaseCreator = (RelationalDatabaseCreator)Database.GetService<IDatabaseCreator>();

                // 检查数据库是否存在
                if (!databaseCreator.Exists()) {
                    // 如果数据库不存在，则创建
                    databaseCreator.Create();
                }
                else {
                    var pendingMigrations = Database.GetPendingMigrations();
                    // 如果有待应用的迁移
                    if (pendingMigrations.Any()) {
                        // 应用待迁移的数据库迁移
                        Database.Migrate();
                    }
                    //如果数据库存在，则判断迁移
                }
            }
            catch (Exception ex) {
                NLog.LogManager.GetCurrentClassLogger().Error($"数据库检查/创建失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            {
                //用户相关
                //用户
                modelBuilder.Entity<UserInfoModel>()
                    .HasKey(c => new {
                        c.Id
                    });
                modelBuilder.Entity<UserInfoModel>()
                    .HasIndex(b => b.UserCode)
                    .IsUnique(true)
                    .IsDescending(true);
                modelBuilder.Entity<UserInfoModel>()
                    .HasIndex(b => b.CreatedTime)
                    .IsUnique(false)
                    .IsDescending(true);
                //用户Vip信息
                modelBuilder.Entity<UserVipInfoModel>()
                    .HasKey(c => new {
                        c.Id
                    });
                //Vip信息关系
                modelBuilder.Entity<UserInfoModel>()
                    .HasOne(b => b.UserVipInfo)
                    .WithOne(n => n.UserInfo)
                    .HasForeignKey<UserVipInfoModel>(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
            {
                //日志相关
                //操作日志
                modelBuilder.Entity<OperationLogInfoModel>()
                    .HasKey(c => new {
                        c.Id
                    });
                modelBuilder.Entity<OperationLogInfoModel>()
                    .HasIndex(b => b.CreatedTime)
                    .IsUnique(false)
                    .IsDescending(true);
                modelBuilder.Entity<OperationLogInfoModel>()
                    .HasIndex(b => b.OperationTime)
                    .IsUnique(false)
                    .IsDescending(true);
                modelBuilder.Entity<OperationLogInfoModel>()
                    .HasIndex(b => b.OperatorId)
                    .IsUnique(false);
                modelBuilder.Entity<OperationLogInfoModel>()
                    .HasIndex(b => b.RequestMethod)
                    .IsUnique(false);
            }

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured) {
                //NLog.LogManager.GetCurrentClassLogger().Warn("DbContextOptions 未配置！");
            }
        }
    }
}