using System.Drawing;

namespace Cropper
{
    /// <summary>
    /// Provides a picture storage abstraction.
    /// </summary>
    public interface IPictureStorage
    {
        /// <summary>
        /// Creates a new file picture on storage with the given id.
        /// </summary>
        void Create(Image image, string id);

        /// <summary>
        /// Gets the image with the given id.
        /// </summary>
        Image Get(string id);
    }
}