using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ToChange
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ToChangeDBModelEntities"].ConnectionString;


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Start Dependecy
            SqlDependency.Start(connectionString);

        }

        protected void Session_Start(object sender , EventArgs e)
        {
            Notification_Adapter.NotificationComponent NC = new Notification_Adapter.NotificationComponent();
            var currentTime = DateTime.Now;
            HttpContext.Current.Session["LastUpdated"] = currentTime;
            NC.RegisterNotification(currentTime);
        }

        protected void Application_End()
        {
            SqlDependency.Stop(connectionString);
        }


    }
}
