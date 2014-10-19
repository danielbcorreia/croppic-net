using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cropper.ModelBinders;
using Cropper.Models;
using SixPack.Diagnostics;

namespace Cropper.Controllers
{
    /*
     * Before you go and use this code in your project, here is what you have to do:
     * - choose the image format that will be saved to disk and change the cropper settings in web.config
     * - if you can, setup an dependency injector and configure ITemporaryStorage<string> to the persistence of your choice. 
     *   if you choose the local filesystem, you can use the LocalTemporaryStorage<string> implementation (configure it as a singleton or static).
     * - setup a logging library so you know when errors happen when cropping.
     */

    public class PictureController : Controller
    {
        // TODO get via dependency injection (singleton scope), and remove the "static" keyword.
        private static readonly ITemporaryStorage<string> _storage = new LocalTemporaryStorage<string>();

        // TODO add your own log implementation
        private static readonly ILog Log = new NullLog();

        // TODO it's always prettier to get this from the dependecy injector
        private static readonly CropperConfigurationSection CropperSettings = MvcApplication.CropperSettings;

        [HttpGet]
        public ActionResult Download(string id)
        {
            return new FileStreamResult(_storage.Open(id), CropperSettings.StoredPictureMediaType);
        }

        [HttpPost]
        public ActionResult Save(HttpPostedFileBase img)
        {
            // check for a valid mediatype
            if (!img.ContentType.StartsWith("image/"))
            {
                return Json(new
                {
                    status = CroppicStatuses.Error,
                    message = "invalid image type"
                });
            }

            // load the image from the upload and generate a new filename
            var image = Image.FromStream(img.InputStream);
            var filename = GenerateFilenameFor(image.Width, image.Height);

            // save to storage
            using (var stream = _storage.Create(filename))
            {
                image.Save(stream, CropperSettings.StoredPictureImageFormatInstance);
            }

            var obj = new
            {
                status = CroppicStatuses.Success,
                url = Url.ActionAbsolute("Download", new { id = filename }),
                width = image.Width,
                height = image.Height
            };

            return Json(obj);
        }

        [HttpPost]
        public ActionResult Crop([ModelBinder(typeof(AliasFormModelBinder))] CropRequest model)
        {
            // extract original image ID and generate a new filename for the cropped result
            var originalUri = new Uri(model.ImageUrl);
            var originalId = originalUri.Segments.Last();
            var croppedId = GenerateFilenameFor(model.CroppedWidth, model.CroppedHeight);

            try
            {
                CropImage(model, originalId, croppedId);
            }
            catch (Exception e)
            {
                Log.HandleException(e);

                return Json(new
                {
                    status = CroppicStatuses.Error,
                    message = "internal error cropping the image"
                });
            }

            return Json(new
            {
                status = CroppicStatuses.Success,
                url = Url.ActionAbsolute("Download", new { id = croppedId })
            });
        }

        private void CropImage(CropRequest model, string originalId, string croppedId)
        {
            // load the original picture and resample it to the scaled values
            Bitmap bitmap;
            using (var originalPicture = _storage.Open(originalId))
            {
                var image = Image.FromStream(originalPicture);
                bitmap = new Bitmap(image, (int)model.ScaledWidth, (int)model.ScaledHeight);
            }

            var croppedBitmap = new Bitmap(model.CroppedWidth, model.CroppedHeight);
            using (var g = Graphics.FromImage(croppedBitmap))
            {
                g.DrawImage(bitmap, 
                    new Rectangle(0, 0, model.CroppedWidth, model.CroppedHeight), 
                    new Rectangle(model.CroppedX, model.CroppedY, model.CroppedWidth, model.CroppedHeight), GraphicsUnit.Pixel);
            }

            // create the cropped picture on storage
            using (var croppedPicture = _storage.Create(croppedId))
            {
                croppedBitmap.Save(croppedPicture, CropperSettings.StoredPictureImageFormatInstance);
            }
        }

        private string GenerateFilenameFor(int width, int height)
        {
            // e.g.: e2384d8f-f24e-47c8-b1d0-d2c286dd9c1b-490x240.png

            return string.Format("{0}-{1}x{2}.{3}", Guid.NewGuid(), width, height, CropperSettings.StoredPictureExtension);
        }
    }

    internal static class CroppicStatuses
    {
        public const string Success = "success";
        public const string Error = "error";
    }
}
