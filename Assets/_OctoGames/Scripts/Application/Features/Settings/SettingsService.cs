using System.Threading;
using System.Threading.Tasks;
using OctoGames.Persistence;

namespace OctoGames.App.Features.Settings
{
    public sealed class SettingsService : ISettingsService
    {
        private readonly IPersistence _persistence;
        private GameSettings _current = GameSettings.CreateDefault();

        public SettingsService(IPersistence persistence)
        {
            _persistence = persistence;
        }

        public GameSettings Current => _current;

        public async Task LoadAsync(CancellationToken ct = default)
        {
            _current = await _persistence.LoadAsync(GameSettings.PersistenceKey, GameSettings.CreateDefault(), ct)
                .ConfigureAwait(false);
        }

        public async Task<PersistenceResult> SaveAsync(CancellationToken ct = default)
        {
            return await _persistence.SaveAsync(GameSettings.PersistenceKey, _current, ct).ConfigureAwait(false);
        }
    }
}
