namespace OctoGames.Persistence
{
    public interface IDataSerializer
    {
        PersistenceResult<string> Serialize<T>(T value);
        PersistenceResult<T> Deserialize<T>(string data);
    }
}
