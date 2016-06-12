using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Web.Mvc;

namespace ChatClient.Infrastructure
{
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public JsonSerializerSettings Settings { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data == null)
            {
                return;
            }

            response.ContentType = String.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

            var serializer = JsonSerializer.Create(Settings);
            serializer.Serialize(response.Output, Data);
        }
    }
}