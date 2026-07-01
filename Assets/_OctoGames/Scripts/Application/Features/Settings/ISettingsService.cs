using System.Threading;
using System.Threading.Tasks;
using OctoGames.Persistence;

namespace OctoGames.App.Features.Settings
{
    public interface ISettingsService
    {
        GameSettings Current { get; }
        Task LoadAsync(CancellationToken ct = default);
        Task<PersistenceResult> SaveAsync(CancellationToken ct = default);
    }
}
