using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FabrikamFiber.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());

            var oldPath = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            var newPath = Path.Combine(oldPath, "..\\..\\FabrikamFiber.Web\\App_Data");
            var collapsedPath = Path.GetFullPath((new Uri(newPath)).LocalPath);
            AppDomain.CurrentDomain.SetData("DataDirectory", collapsedPath);
        }
    }
}
