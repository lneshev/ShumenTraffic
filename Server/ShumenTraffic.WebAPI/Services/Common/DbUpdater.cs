using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Data.Context;
using System.Threading.Tasks;

namespace ShumenTraffic.WebAPI.Services.Common
{
    public class DbUpdater : IDbUpdater
    {
        private readonly ShumenTrafficDbContext appDbContext;

        public DbUpdater(ShumenTrafficDbContext appDbContext)
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