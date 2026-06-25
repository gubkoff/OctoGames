using VContainer;
using VContainer.Unity;

namespace OctoGames.Composition
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            PersistenceRegistration.Register(builder);
            SettingsRegistration.Register(builder);
        }
    }
}
