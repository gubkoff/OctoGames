using System.Threading;
using System.Threading.Tasks;
using OctoGames.Persistence;

namespace OctoGames.Gameplay.Features.Settings
{
    public sealed class SettingsService : ISettingsService
    {
        readonly IPersistence _persistence;
        GameSettings _current = GameSettings.CreateDefault();

        public SettingsService(IPersistence persistence)
        {
            _persistence = persistence;
        }

        public GameSettings Current => _current;

        public async Task LoadAsync(CancellationToken ct = default)
        {
            _current = await _persistence.LoadAsync(GameSettings.PersistenceKey, GameSettings.CreateDefault(), ct)
                .ConfigureAwait(false);
            _current.Normalize();
        }

        public async Task<PersistenceResult> SaveAsync(CancellationToken ct = default)
        {
            _current.Normalize();
            return await _persistence.SaveAsync(GameSettings.PersistenceKey, _current, ct).ConfigureAwait(false);
        }
    }
}
