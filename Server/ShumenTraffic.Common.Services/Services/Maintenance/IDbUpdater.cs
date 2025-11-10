using ShumenTraffic.Common.Core.DTOs.Maintenance;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Maintenance
{
    public interface IDbUpdater
    {
        Task<DbsUpdateResult> CreateAndUpdateAllAsync();
    }
}