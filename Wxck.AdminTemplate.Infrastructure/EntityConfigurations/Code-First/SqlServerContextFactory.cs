using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;

namespace Wxck.AdminTemplate.Infrastructure.EntityConfigurations.Code_First {

    public class SqlServerContextFactory : IDesignTimeDbContextFactory<SqlServerContext> {

        public SqlServerContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerContext>();

            // 更新为 SQL Server 的连接字符串
            //var connectionString = "server=154.204.45.171,1433;uid=adminnet;pwd=JinYu6688;database=TestDB;Encrypt=true;TrustServerCertificate=true;";
            // 设置配置文件的路径，直接从当前目录加载 appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // 会从 bin/Debug 下读取
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            Debug.WriteLine(connectionString);
            // 使用 UseSqlServer 配置 SQL Server
            optionsBuilder.UseSqlServer(connectionString, options => {
                // 这里您可以继续设置其他选项，比如 SchemaBehavior
                // SQL Server 并没有 MySQL 的 SchemaBehavior，但您可以根据需要进行额外配置
            });

            return new SqlServerContext(optionsBuilder.Options);
        }
    }
}