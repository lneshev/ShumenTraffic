using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Maintenance
{
    public interface IDbUpdater
    {
        Task CreateAndUpdateAsync();
    }
}