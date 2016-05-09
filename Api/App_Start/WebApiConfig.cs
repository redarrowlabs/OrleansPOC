using Newtonsoft.Json.Serialization;
using Orleans;
using Orleans.Runtime.Configuration;
using System.Web.Http;

namespace Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var orleansConfig = ClientConfiguration.LocalhostSilo();
            GrainClient.Initialize(orleansConfig);

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}