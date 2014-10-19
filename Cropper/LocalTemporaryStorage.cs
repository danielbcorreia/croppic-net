using System;
using System.Configuration;
using System.Linq;
using System.IO;

namespace Cropper
{
    public class LocalTemporaryStorage<TKey> : ITemporaryStorage<TKey>
    {
        private readonly string _basePath;

        public LocalTemporaryStorage()
        {
            var configuredBasePath = ConfigurationManager.AppSettings["LocalTemporaryStorageFolder"];

            _basePath = configuredBasePath ?? Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_basePath);
        }

        public virtual Stream Create(TKey key)
        {
            var filename = key.ToString().ToCharArray();
            var invalidChars = Path.GetInvalidFileNameChars();

            // avoid invalid chars in the filename
            filename = filename
                .Where(x => !invalidChars.Contains(x))
                .ToArray();

            var stream = File.Create(Path.Combine(_basePath, new string(filename)));
            return stream;
        }

        public virtual Stream Open(TKey key)
        {
            var stream = File.OpenRead(Path.Combine(_basePath, key.ToString()));
            return stream;
        }
    }
}