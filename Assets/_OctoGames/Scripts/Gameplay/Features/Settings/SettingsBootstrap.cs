using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace OctoGames.Gameplay.Features.Settings
{
    public sealed class SettingsBootstrap : IAsyncStartable
    {
        readonly ISettingsService _settings;

        public SettingsBootstrap(ISettingsService settings)
        {
            _settings = settings;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            await _settings.LoadAsync(cancellation);
        }
    }
}
