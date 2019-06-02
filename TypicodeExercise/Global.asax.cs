using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Serilog;

namespace TypicodeExercise
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            Log.Information("Application started, global logger configured successfully");
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // don't include detailed error messages thrown by the framework if not running locally
            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

        }


    }
}
