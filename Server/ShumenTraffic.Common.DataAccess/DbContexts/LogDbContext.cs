using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ShumenTraffic.Common.DataAccess.DbContexts
{
    /// <summary>
    /// A DbContext for working with "Log" database.
    /// </summary>
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CS_AS");
        }
    }
}