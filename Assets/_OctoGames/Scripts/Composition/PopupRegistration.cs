using System;
using OctoGames.Popups;
using VContainer;

namespace OctoGames.Composition
{
    public static class PopupRegistration
    {
        public static void Register(
            IContainerBuilder builder,
            IPopupProvider provider,
            PopupRoot root)
        {
            if (provider == null)
                throw new InvalidOperationException(
                    "IPopupProvider is not assigned. Assign Popups Catalog on GameLifetimeScope.");

            if (root == null)
                throw new InvalidOperationException(
                    "PopupRoot is not available. Assign PopupRoot on GameLifetimeScope.");

            builder.RegisterInstance(provider);
            builder.RegisterInstance(root);
            builder.Register<IPopupService, PopupService>(Lifetime.Singleton);
        }
    }
}
