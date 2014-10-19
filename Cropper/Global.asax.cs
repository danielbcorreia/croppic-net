using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cropper
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static CropperConfigurationSection CropperSettings { get; set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            CropperSettings = (CropperConfigurationSection)ConfigurationManager.GetSection("cropper");
        }

        protected void Application_PreSendRequestHeaders()
        {
            if (!string.IsNullOrEmpty(CropperSettings.AllowedCrossOriginRequests))
            {
                // allow cross-origin requests from the configured origin. this allows us to run the client code on a different domain.
                Response.Headers.Set("Access-Control-Allow-Origin", CropperSettings.AllowedCrossOriginRequests);
            }
        }
    }
}
