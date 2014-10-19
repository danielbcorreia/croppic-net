using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Cropper
{
    /// <summary>
    /// Provides a temporary storage abstraction.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface ITemporaryStorage<TKey>
    {
        /// <summary>
        /// Creates a new file on the shared storage.
        /// </summary>
        Stream Create(TKey key);

        /// <summary>
        /// Gets the file stream associated by the specified key.
        /// </summary>
        Stream Open(TKey key);
    }
}