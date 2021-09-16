using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DatingAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            config.Routes.MapHttpRoute(
                name: "Root",
                routeTemplate: "",
                defaults: new { controller = "Dating", action = "Index" }

            );

            config.Routes.MapHttpRoute(
                 name: "Error404",
                routeTemplate: "{*url}",
                defaults: new { controller = "Error", action = "Handle404" }
                );
        }
    }
}
