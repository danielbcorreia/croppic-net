using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace Cropper.ModelBinders
{
    public class AliasFormModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor)
        {
            var aliasAttribute = TryGetAttribute<AliasAttribute>(propertyDescriptor);
            if (aliasAttribute != null)
            {
                var strValue = controllerContext.HttpContext.Request.Form[aliasAttribute.Name];
                object value;

                try
                {
                    value = Convert.ChangeType(strValue, propertyDescriptor.PropertyType);
                }
                catch (FormatException e)
                {
                    // small hack to round floats to integers, needed because the original cropper JS does not handle half-pixel values...
                    if (propertyDescriptor.PropertyType == typeof (int) && strValue.Contains("."))
                    {
                        value = (int)Math.Round(Convert.ToDouble(strValue));
                    }
                    else
                    {
                        throw;
                    }
                }

                propertyDescriptor.SetValue(bindingContext.Model, value);
            }
            else
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }

        private T TryGetAttribute<T>(PropertyDescriptor propertyDescriptor) where T : Attribute
        {
            return propertyDescriptor.Attributes
              .OfType<T>()
              .FirstOrDefault();
        }
    }
}