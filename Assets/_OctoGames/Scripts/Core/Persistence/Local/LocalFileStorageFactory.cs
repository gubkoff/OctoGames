using System.IO;
using OctoGames.Persistence.Providers;
using UnityEngine;

namespace OctoGames.Persistence.Local
{
    public static class LocalFileStorageFactory
    {
        public static string GetRootPath(string subfolder = "saves") =>
            Path.Combine(Application.persistentDataPath, subfolder);

        public static JsonStorageProvider CreateDefault(string subfolder = "saves") =>
            new(GetRootPath(subfolder));
    }
}
