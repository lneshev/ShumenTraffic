using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.DataAccess.DbContexts;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Maintenance
{
    public class DbUpdater : IDbUpdater
    {
        private readonly AppDbContext appDbContext;

        public DbUpdater(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task CreateAndUpdateAsync()
        {
            await MigrateAndSeedAppDB();
        }

        private async Task MigrateAndSeedAppDB()
        {
            await appDbContext.Database.MigrateAsync();
        }
    }
}