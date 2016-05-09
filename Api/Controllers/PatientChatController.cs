using Api.Models;
using Common;
using GrainInterfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Controllers
{
    [RoutePrefix("patient/chat")]
    public class PatientChatController : ApiController
    {
        [HttpGet]
        [Route("messages/{id:long}")]
        public Task<IEnumerable<ChatMessage>> Messages(long id)
        {
            var patient = GrainClient.GrainFactory.GetGrain<IPatientGrain>(id);
            return patient.Messages();
        }

        [HttpPost]
        [Route("messages")]
        public async Task<IHttpActionResult> SendMessage(PatientChatMessage message)
        {
            var patient = GrainClient.GrainFactory.GetGrain<IPatientGrain>(message.Id);
            await patient.SendMessage(
                new ChatMessage
                {
                    Name = "Patient" + message.Id,
                    Text = message.Message,
                    Received = DateTime.UtcNow
                }
            );

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}