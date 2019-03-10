using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApiPaginatedCrud
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            /*
             *  
             var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
              formatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
             */


            config.EnableCors();

            // var cors = new EnableCorsAttribute(origins: "*", headers: "*", methods: "*");
            // config.EnableCors(cors);

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };

            // Web API routes
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
            );
        }
    }
}