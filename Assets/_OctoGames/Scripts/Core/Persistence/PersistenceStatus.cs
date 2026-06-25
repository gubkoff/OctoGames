namespace OctoGames.Persistence
{
    public enum PersistenceStatus
    {
        Success,
        NotFound,
        Corrupted,
        SerializationError,
        IoError,
        Cancelled
    }
}
