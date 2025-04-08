using Microsoft.EntityFrameworkCore;

namespace Wxck.AdminTemplate.Infrastructure.EntityConfigurations {

    public sealed class SqliteConfContext : DbContext {

        public SqliteConfContext(DbContextOptions<SqliteConfContext> options) : base(options) {
        }
    }
}