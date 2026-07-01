namespace OctoGames.Persistence
{
    public readonly struct PersistenceResult
    {
        public PersistenceStatus Status { get; }

        public bool IsSuccess => Status == PersistenceStatus.Success;

        private PersistenceResult(PersistenceStatus status) => Status = status;

        public static PersistenceResult Ok() => new(PersistenceStatus.Success);

        public static PersistenceResult Fail(PersistenceStatus status) => new(status);
    }

    public readonly struct PersistenceResult<T>
    {
        public PersistenceStatus Status { get; }
        public T Value { get; }

        public bool IsSuccess => Status == PersistenceStatus.Success;

        private PersistenceResult(PersistenceStatus status, T value)
        {
            Status = status;
            Value = value;
        }

        public static PersistenceResult<T> Ok(T value) => new(PersistenceStatus.Success, value);

        public static PersistenceResult<T> Fail(PersistenceStatus status, T value = default) =>
            new(status, value);
    }
}
