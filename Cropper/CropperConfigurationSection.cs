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

        public string AllowedCrossOriginRequests { get; set; }
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
        /// Allow cross-origin requests from the specified origins
        /// </summary>
        string AllowedCrossOriginRequests { get; }
    }
}