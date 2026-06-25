using System.Threading;
using System.Threading.Tasks;

namespace OctoGames.Persistence
{
    public interface IStorageProvider
    {
        Task<PersistenceResult> SaveAsync(string key, string data, CancellationToken ct = default);
        Task<PersistenceResult<string>> LoadAsync(string key, CancellationToken ct = default);
        Task<bool> ExistsAsync(string key, CancellationToken ct = default);
        Task<PersistenceResult> DeleteAsync(string key, CancellationToken ct = default);
    }
}
