using Common;
using GrainInterfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Controllers
{
    [RoutePrefix("chat/{id:Guid}")]
    public class ChatController : ApiController
    {
        [HttpGet]
        [Route("users")]
        public Task<IEnumerable<Entity>> Users(Guid id)
        {
            var chat = GrainClient.GrainFactory.GetGrain<IChatGrain>(id);

            return chat.Users();
        }

        [HttpGet]
        [Route("messages")]
        public Task<IEnumerable<ChatMessage>> Messages(Guid id)
        {
            var chat = GrainClient.GrainFactory.GetGrain<IChatGrain>(id);

            return chat.Messages();
        }
    }
}