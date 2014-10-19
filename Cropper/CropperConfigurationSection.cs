using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing.Imaging;
using System.Reflection;

namespace Cropper
{
    public class CropperConfigurationSection : ICropperConfigurationSection
    {
        public bool ForceHtmlContentTypeInResponses { get; set; }

        public string StoredPictureMediaType { get; set; }

        public string StoredPictureExtension { get; set; }

        public string StoredPictureImageFormat { get; set; }

        public string AllowedCrossOriginRequests { get; set; }

        // read only, parsed from other properties

        private ImageFormat _imageFormatInstance;
        public ImageFormat StoredPictureImageFormatInstance
        {
            get
            {
                if (_imageFormatInstance != null)
                {
                    return _imageFormatInstance;
                }

                var property = typeof(ImageFormat)
                    .GetProperty(StoredPictureImageFormat, BindingFlags.Public | BindingFlags.Static);

                if (property == null)
                {
                    throw new ConfigurationErrorsException("Invalid ImageFormat name");
                }

                return _imageFormatInstance = (ImageFormat)property.GetValue(null);
            }
        }
    }

    public interface ICropperConfigurationSection
    {
        /// <summary>
        /// This is needed for the original version of the Cropper plugin, since it does not support JSON responses.
        /// To avoid using this switch, change the following line (appears 2 times in the file):
        /// "response = jQuery.parseJSON(data)" to "response = typeof data === 'string' ? jQuery.parseJSON(data) : data"
        /// 
        /// This way, if jQuery gives us a JS object directly (meaning that it already parsed the JSON from the server), 
        /// then we don't need to parse it again (and we would also get an error).
        /// </summary>
        bool ForceHtmlContentTypeInResponses { get; }

        /// <summary>
        /// The mediatype of the images that are stored. E.g.: image/png
        /// </summary>
        string StoredPictureMediaType { get; }

        /// <summary>
        /// The extension of the images that are stored, E.g.: png
        /// </summary>
        string StoredPictureExtension { get; }

        /// <summary>
        /// The System.Drawing.Imaging.ImageFormat name of the images that are stored, E.g.: Png
        /// </summary>
        string StoredPictureImageFormat { get; }

        /// <summary>
        /// The System.Drawing.Imaging.ImageFormat instance of the images that are stored, E.g.: ImageFormat.Png
        /// </summary>
        ImageFormat StoredPictureImageFormatInstance { get; }

        /// <summary>
        /// Allow cross-origin requests from the specified origins
        /// </summary>
        string AllowedCrossOriginRequests { get; }
    }
}