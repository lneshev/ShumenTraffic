using System.Threading.Tasks;

namespace ShumenTraffic.WebAPI.Services.Common
{
    public interface IDbUpdater
    {
        Task CreateAndUpdateAsync();
    }
}