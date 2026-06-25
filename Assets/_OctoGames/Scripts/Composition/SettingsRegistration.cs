using OctoGames.Gameplay.Features.Settings;
using VContainer;
using VContainer.Unity;

namespace OctoGames.Composition
{
    public static class SettingsRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<ISettingsService, SettingsService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SettingsBootstrap>();
        }
    }
}
