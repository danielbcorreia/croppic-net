using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Cropper
{
    public class ImageResult : ActionResult
    {
        private readonly Image _image;
        private readonly string _name;

        public ImageResult(Image image, string name)
        {
            _image = image;
            _name = name;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var ms = new MemoryStream();
            _image.Save(ms, _image.RawFormat);
            ms.Position = 0;

            var fileStreamResult = new FileStreamResult(ms, MimeMapping.GetMimeMapping(_name));
            fileStreamResult.ExecuteResult(context);
        }
    }
}