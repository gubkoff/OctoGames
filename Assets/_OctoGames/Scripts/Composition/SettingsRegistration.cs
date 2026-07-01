using OctoGames.App.Features.Settings;
using OctoGames.App.Features.Settings.Popups.ViewModels;
using VContainer;

namespace OctoGames.Composition
{
    public static class SettingsRegistration
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<ISettingsService, SettingsService>(Lifetime.Singleton);
            builder.Register<SettingsPopupViewModel>(Lifetime.Transient);
        }
    }
}
