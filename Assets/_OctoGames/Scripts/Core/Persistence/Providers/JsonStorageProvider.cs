using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OctoGames.Persistence.Providers
{
    public sealed class JsonStorageProvider : IStorageProvider
    {
        readonly string _rootDirectory;

        public JsonStorageProvider(string rootDirectory)
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
                throw new ArgumentException("Root directory is required.", nameof(rootDirectory));

            _rootDirectory = rootDirectory;
        }

        public async Task<PersistenceResult> SaveAsync(string key, string data, CancellationToken ct = default)
        {
            var filePath = GetFilePath(key);
            var tempPath = filePath + ".tmp";

            try
            {
                Directory.CreateDirectory(_rootDirectory);

                await using (var stream = new FileStream(
                                 tempPath,
                                 FileMode.Create,
                                 FileAccess.Write,
                                 FileShare.None,
                                 bufferSize: 4096,
                                 options: FileOptions.Asynchronous))
                await using (var writer = new StreamWriter(stream))
                {
                    ct.ThrowIfCancellationRequested();
                    await writer.WriteAsync(data.AsMemory(), ct).ConfigureAwait(false);
                    await writer.FlushAsync().ConfigureAwait(false);
                    await stream.FlushAsync(ct).ConfigureAwait(false);
                }

                ct.ThrowIfCancellationRequested();

                if (File.Exists(filePath))
                    File.Replace(tempPath, filePath, destinationBackupFileName: null);
                else
                    File.Move(tempPath, filePath);

                return PersistenceResult.Ok();
            }
            catch (OperationCanceledException)
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
                return PersistenceResult.Fail(PersistenceStatus.Cancelled);
            }
            catch (Exception)
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
                return PersistenceResult.Fail(PersistenceStatus.IoError);
            }
        }

        public async Task<PersistenceResult<string>> LoadAsync(string key, CancellationToken ct = default)
        {
            var filePath = GetFilePath(key);
            if (!File.Exists(filePath))
                return PersistenceResult<string>.Fail(PersistenceStatus.NotFound);

            try
            {
                var data = await File.ReadAllTextAsync(filePath, ct).ConfigureAwait(false);
                return PersistenceResult<string>.Ok(data);
            }
            catch (OperationCanceledException)
            {
                return PersistenceResult<string>.Fail(PersistenceStatus.Cancelled);
            }
            catch (Exception)
            {
                return PersistenceResult<string>.Fail(PersistenceStatus.IoError);
            }
        }

        public Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            return Task.FromResult(File.Exists(GetFilePath(key)));
        }

        public Task<PersistenceResult> DeleteAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var filePath = GetFilePath(key);
            if (!File.Exists(filePath))
                return Task.FromResult(PersistenceResult.Ok());

            try
            {
                File.Delete(filePath);
                return Task.FromResult(PersistenceResult.Ok());
            }
            catch (Exception)
            {
                return Task.FromResult(PersistenceResult.Fail(PersistenceStatus.IoError));
            }
        }

        string GetFilePath(string key) =>
            Path.Combine(_rootDirectory, SanitizeKey(key) + ".json");

        static string SanitizeKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key is required.", nameof(key));

            var sanitized = key;
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
                sanitized = sanitized.Replace(invalidChar, '_');

            return sanitized.Replace('/', '_').Replace('\\', '_');
        }
    }
}
