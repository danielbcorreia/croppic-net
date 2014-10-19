using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
     * - if you can, setup an dependency injector and configure IPictureStorage to the persistence of your choice. 
     *   if you choose the local filesystem, you can use the FileSystemTemporaryPictureStorage implementation (configure it as a singleton or static).
     * - setup a logging library so you know when errors happen when cropping.
     */

    public class PictureController : Controller
    {
        // TODO get via dependency injection (singleton scope), and remove the "static" keyword.
        private static readonly IPictureStorage _storage = new FileSystemPictureStorage();

        // TODO add your own log implementation
        private static readonly ILog Log = new NullLog();

        // TODO it's always prettier to get this from the dependecy injector
        private static readonly CropperConfigurationSection CropperSettings = MvcApplication.CropperSettings;

        [HttpGet]
        public ActionResult Download(string id)
        {
            var image = _storage.Get(id);
            return new ImageResult(image, id);
        }

        [HttpPost]
        public ActionResult Save(HttpPostedFileBase img)
        {
            // check for a valid mediatype
            if (!img.ContentType.StartsWith("image/"))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return Json(new
                {
                    status = CroppicStatuses.Error,
                    message = "invalid image type"
                });
            }

            // load the image from the upload and generate a new filename
            var image = Image.FromStream(img.InputStream);
            var extension = Path.GetExtension(img.FileName);
            var id = GenerateIdFor(image.Width, image.Height, extension);

            // save to storage
            _storage.Create(image, id);

            var obj = new
            {
                status = CroppicStatuses.Success,
                url = Url.ActionAbsolute("Download", new { id }),
                width = image.Width,
                height = image.Height
            };

            return SerializedObject(obj);
        }

        [HttpPost]
        public ActionResult Crop([ModelBinder(typeof(AliasFormModelBinder))] CropRequest model)
        {
            // extract original image ID and generate a new filename for the cropped result
            var originalUri = new Uri(model.ImageUrl);
            var originalId = originalUri.Segments.Last();
            var extension = Path.GetExtension(originalId);
            var croppedId = GenerateIdFor(model.CroppedWidth, model.CroppedHeight, extension);

            try
            {
                CropImage(model, originalId, croppedId);
            }
            catch (Exception e)
            {
                Log.HandleException(e);

                Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                return SerializedObject(new
                {
                    status = CroppicStatuses.Error,
                    message = "internal error cropping the image"
                });
            }

            return SerializedObject(new
            {
                status = CroppicStatuses.Success,
                url = Url.ActionAbsolute("Download", new { id = croppedId })
            });
        }

        private void CropImage(CropRequest model, string originalId, string croppedId)
        {
            // load the original picture and resample it to the scaled values
            Bitmap bitmap;
            using (var image = _storage.Get(originalId))
            {
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
            _storage.Create(croppedBitmap, croppedId);
        }

        private string GenerateIdFor(int width, int height, string extension)
        {
            // e.g.: e2384d8f-f24e-47c8-b1d0-d2c286dd9c1b-490x240.png

            return string.Format("{0}-{1}x{2}.{3}", Guid.NewGuid(), width, height, extension);
        }

        private ActionResult SerializedObject(object obj)
        {
            var result = Json(obj);

            if (CropperSettings.ForceHtmlContentTypeInResponses)
            {
                result.ContentType = "text/html";
            }

            return result;
        }
    }

    internal static class CroppicStatuses
    {
        public const string Success = "success";
        public const string Error = "error";
    }
}
