namespace OctoGames.Spawn
{
    public abstract class BaseSpawner<T, TSpawnData> : ISpawn<T, TSpawnData> where T : class
    {
        public abstract T Spawn(TSpawnData data);

        public void Despawn(T item)
        {
            if (item == null)
                return;

            OnDespawn(item);
        }

        protected abstract void OnDespawn(T item);
    }
}
