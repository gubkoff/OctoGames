using System.Text.Json;
using OctoGames.Persistence;
using OctoGames.Persistence.Providers;
using OctoGames.Persistence.Serialization;
using OctoGames.Persistence.Local;
using VContainer;

namespace OctoGames.Composition
{
    public static class PersistenceRegistration
    {
        public static void Register(IContainerBuilder builder, string savesSubfolder = "saves")
        {
            RegisterWithRoot(builder, LocalFileStorageFactory.GetRootPath(savesSubfolder));
        }

        public static void RegisterWithRoot(IContainerBuilder builder, string rootDirectory)
        {
            builder.Register<IDataSerializer>(resolver =>
            {
                if (resolver.TryResolve<JsonSerializerOptions>(out var options))
                    return new SystemTextJsonDataSerializer(options);
                return new SystemTextJsonDataSerializer();
            }, Lifetime.Singleton);
            builder.Register<IStorageProvider>(
                _ => new JsonStorageProvider(rootDirectory),
                Lifetime.Singleton);
            builder.Register<IPersistence, PersistenceService>(Lifetime.Singleton);
        }
    }
}
