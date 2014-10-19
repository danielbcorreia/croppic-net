using System;
using System.Configuration;
using System.Drawing;
using System.IO;

namespace Cropper
{
    public class FileSystemPictureStorage : IPictureStorage
    {
        private readonly string _basePath;

        public FileSystemPictureStorage()
        {
            var configuredBasePath = ConfigurationManager.AppSettings["FileSystemPictureStorageFolder"];

            _basePath = configuredBasePath ?? Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_basePath);
        }

        public void Create(Image image, string filename)
        {
            image.Save(Path.Combine(_basePath, filename));
        }

        public Image Get(string filename)
        {
            return Image.FromFile(Path.Combine(_basePath, filename));
        }
    }
}