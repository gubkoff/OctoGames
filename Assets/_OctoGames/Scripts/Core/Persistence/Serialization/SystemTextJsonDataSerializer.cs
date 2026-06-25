using System;
using System.Text.Json;

namespace OctoGames.Persistence.Serialization
{
    public sealed class SystemTextJsonDataSerializer : IDataSerializer
    {
        readonly JsonSerializerOptions _options;

        public SystemTextJsonDataSerializer(JsonSerializerOptions options = null)
        {
            _options = options ?? CreateDefaultOptions();
        }

        public static JsonSerializerOptions CreateDefaultOptions() => new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public PersistenceResult<string> Serialize<T>(T value)
        {
            try
            {
                return PersistenceResult<string>.Ok(JsonSerializer.Serialize(value, _options));
            }
            catch (Exception)
            {
                return PersistenceResult<string>.Fail(PersistenceStatus.SerializationError);
            }
        }

        public PersistenceResult<T> Deserialize<T>(string data)
        {
            try
            {
                return PersistenceResult<T>.Ok(JsonSerializer.Deserialize<T>(data, _options));
            }
            catch (Exception)
            {
                return PersistenceResult<T>.Fail(PersistenceStatus.Corrupted);
            }
        }
    }
}
