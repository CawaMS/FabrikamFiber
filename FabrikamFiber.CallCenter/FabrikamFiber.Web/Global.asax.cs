namespace FabrikamFiber.Web
{
    using System.Data.Entity;
    using System.Web.Mvc;
    using System.Web.Routing;

    using FabrikamFiber.DAL.Data;
    using System;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using System.Collections.Generic;
    using FabrikamFiber.Web.ViewModels;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static TelemetryClient AppInsights;

        public static Dictionary<int, DashboardViewModel> ViewModelCache { get; set; } = new Dictionary<int, DashboardViewModel>();

        public static int CacheID = 0;
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_BeginRequest()
        {
            System.Diagnostics.Trace.WriteLine("New Request Received");
        }

        protected void Application_Start()
        {
            Database.SetInitializer<FabrikamFiberWebContext>(new FabrikamFiberDatabaseInitializer());

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            TelemetryClient tc = new TelemetryClient();

            MvcApplication.AppInsights = new TelemetryClient();

            // Register telemetry processors
            var builder = TelemetryConfiguration.Active.TelemetryProcessorChainBuilder;
            builder.Use((next) => new AIExtensions.StaticContentFilter(next));
            builder.Build();

            
            tc.TrackMetric("Customer Lead Time", new Random().NextDouble()*1000);

            try
            {
                int a = 9, b = new Random().Next(2);
                a = a/b;
            }
            catch (Exception e)
            {
                tc.TrackException(e);
            }
            
            tc.TrackTrace("Application loaded successfully");
            var complexEvent = new Microsoft.ApplicationInsights.DataContracts.EventTelemetry();
            complexEvent.Name = "Customer Load";
            if (new Random().Next(2) == 0)
                complexEvent.Properties["Section"] = "A";
            else 
                complexEvent.Properties["Section"] = "B";
            complexEvent.Metrics["Completion Time"] = new Random().Next(43);
            tc.Track(complexEvent);
        }
    }
}