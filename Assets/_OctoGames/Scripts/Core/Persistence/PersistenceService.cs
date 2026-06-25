using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoGames.Persistence
{
    public sealed class PersistenceService : IPersistence
    {
        readonly IStorageProvider _provider;
        readonly IDataSerializer _serializer;
        readonly SemaphoreSlim _writeLock = new(1, 1);

        public PersistenceService(IStorageProvider provider, IDataSerializer serializer)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task<PersistenceResult> SaveAsync<T>(string key, T data, CancellationToken ct = default)
        {
            var serialized = _serializer.Serialize(data);
            if (!serialized.IsSuccess)
                return PersistenceResult.Fail(serialized.Status);

            await _writeLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                return await _provider.SaveAsync(key, serialized.Value, ct).ConfigureAwait(false);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public async Task<T> LoadAsync<T>(string key, T defaultValue = default, CancellationToken ct = default)
        {
            var result = await TryLoadAsync<T>(key, ct).ConfigureAwait(false);
            return result.IsSuccess ? result.Value : defaultValue;
        }

        public async Task<PersistenceResult<T>> TryLoadAsync<T>(string key, CancellationToken ct = default)
        {
            var load = await _provider.LoadAsync(key, ct).ConfigureAwait(false);
            if (!load.IsSuccess)
                return PersistenceResult<T>.Fail(load.Status);

            return _serializer.Deserialize<T>(load.Value);
        }

        public Task<bool> ExistsAsync(string key, CancellationToken ct = default) =>
            _provider.ExistsAsync(key, ct);

        public async Task<PersistenceResult> DeleteAsync(string key, CancellationToken ct = default)
        {
            await _writeLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                return await _provider.DeleteAsync(key, ct).ConfigureAwait(false);
            }
            finally
            {
                _writeLock.Release();
            }
        }
    }
}
