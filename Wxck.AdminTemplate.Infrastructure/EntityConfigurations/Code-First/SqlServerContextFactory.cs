using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Wxck.AdminTemplate.Infrastructure.EntityConfigurations.DB_First {

    public class SqlServerContextFactory : IDesignTimeDbContextFactory<SqlServerContext> {

        public SqlServerContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerContext>();

            // 更新为 SQL Server 的连接字符串
            var connectionString = "server=154.204.45.171,1433;uid=adminnet;pwd=JinYu6688;database=TestDB;Encrypt=true;TrustServerCertificate=true;";

            // 使用 UseSqlServer 配置 SQL Server
            optionsBuilder.UseSqlServer(connectionString, options => {
                // 这里您可以继续设置其他选项，比如 SchemaBehavior
                // SQL Server 并没有 MySQL 的 SchemaBehavior，但您可以根据需要进行额外配置
            });

            return new SqlServerContext(optionsBuilder.Options);
        }
    }
}