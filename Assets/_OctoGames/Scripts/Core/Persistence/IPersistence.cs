using System.Threading;
using System.Threading.Tasks;

namespace OctoGames.Persistence
{
    public interface IPersistence
    {
        Task<PersistenceResult> SaveAsync<T>(string key, T data, CancellationToken ct = default);
        Task<T> LoadAsync<T>(string key, T defaultValue = default, CancellationToken ct = default);
        Task<PersistenceResult<T>> TryLoadAsync<T>(string key, CancellationToken ct = default);
        Task<bool> ExistsAsync(string key, CancellationToken ct = default);
        Task<PersistenceResult> DeleteAsync(string key, CancellationToken ct = default);
    }
}
