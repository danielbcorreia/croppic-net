using Cropper.ModelBinders;

namespace Cropper.Models
{
    public class CropRequest
    {
        /// <summary>
        /// your image path (the one we recieved after successfull upload)
        /// </summary>
        [Alias("imgUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// your image original width (the one we recieved after upload)
        /// </summary>
        [Alias("imgInitW")]
        public int OriginalWidth { get; set; }

        /// <summary>
        /// your image original height (the one we recieved after upload)
        /// </summary>
        [Alias("imgInitH")]
        public int OriginalHeight { get; set; }

        /// <summary>
        /// your new scaled image width
        /// </summary>
        [Alias("imgW")]
        public double ScaledWidth { get; set; }

        /// <summary>
        /// your new scaled image height
        /// </summary>
        [Alias("imgH")]
        public double ScaledHeight { get; set; }

        /// <summary>
        /// top left corner of the cropped image in relation to scaled image
        /// </summary>
        [Alias("imgX1")]
        public int CroppedX { get; set; }

        /// <summary>
        /// top left corner of the cropped image in relation to scaled image
        /// </summary>
        [Alias("imgY1")]
        public int CroppedY { get; set; }

        /// <summary>
        /// cropped image width
        /// </summary>
        [Alias("cropW")]
        public int CroppedWidth { get; set; }

        /// <summary>
        /// cropped image height
        /// </summary>
        [Alias("cropH")]
        public int CroppedHeight { get; set; }
    }
}