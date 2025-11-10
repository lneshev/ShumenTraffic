using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using ShumenTraffic.Common.Core.DTOs.Maintenance;
using ShumenTraffic.Common.Core.Enums.Maintenance;
using ShumenTraffic.Common.DataAccess.DbContexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Maintenance
{
    public class DbUpdater : IDbUpdater
    {
        private readonly LogDbContext logDbContext;
        private readonly AppDbContext appDbContext;

        public DbUpdater(LogDbContext logDbContext, AppDbContext appDbContext)
        {
            this.logDbContext = logDbContext;
            this.appDbContext = appDbContext;
        }

        public async Task<DbsUpdateResult> CreateAndUpdateAllAsync()
        {
            var dbsUpdateResult = new DbsUpdateResult();
            dbsUpdateResult.Results.AddRange(new[]
            {
                new DbUpdateResult() { Name = "Log" },
                new DbUpdateResult() { Name = "App" }
            });

            try
            {
                await MigrateLogDB();
                dbsUpdateResult.Results.Single(x => x.Name == "Log").State = DbUpdateState.Success;
            }
            catch (Exception ex)
            {
                DbUpdateResult dbUpdateResult = dbsUpdateResult.Results.Single(x => x.Name == "Log");
                dbUpdateResult.State = DbUpdateState.Fail;
                dbUpdateResult.Exception = ex;
                dbsUpdateResult.State = DbsUpdateState.FailNoActionNeeded;
            }

            if (dbsUpdateResult.State == DbsUpdateState.Unknown)
            {
                try
                {
                    await MigrateAndSeedAppDB();
                    dbsUpdateResult.Results.Single(x => x.Name == "App").State = DbUpdateState.Success;
                }
                catch (Exception ex)
                {
                    DbUpdateResult dbUpdateResult = dbsUpdateResult.Results.Single(x => x.Name == "App");
                    dbUpdateResult.State = DbUpdateState.Fail;
                    dbUpdateResult.Exception = ex;
                    dbsUpdateResult.State = DbsUpdateState.FailNoActionNeeded;
                }
            }

            if (dbsUpdateResult.State == DbsUpdateState.Unknown)
            {
                dbsUpdateResult.State = DbsUpdateState.Success;
            }

            return dbsUpdateResult;
        }

        private async Task MigrateLogDB()
        {
            var logDbCreator = (IRelationalDatabaseCreator)logDbContext.GetInfrastructure().GetRequiredService<IDatabaseCreator>();
            if (!await logDbCreator.ExistsAsync())
            {
                await logDbCreator.CreateAsync();
            }
        }

        private async Task MigrateAndSeedAppDB()
        {
            await appDbContext.Database.MigrateAsync();
        }
    }
}