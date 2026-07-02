namespace OctoGames.Spawn
{
    public interface ISpawn<T, in TSpawnData> where T : class
    {
        T Spawn(TSpawnData data);
        void Despawn(T item);
    }
}
