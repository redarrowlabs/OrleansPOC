using Api.Models;
using Common;
using GrainInterfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Controllers
{
    [RoutePrefix("provider/chat")]
    public class ProviderChatController : ApiController
    {
        [HttpGet]
        [Route("patients/{id:long}")]
        public async Task<IEnumerable<Patient>> Patients(long id)
        {
            var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(id);
            var patientGrains = await provider.CurrentPatients();

            return patientGrains.Select(x => new Patient
            {
                Id = x.GetPrimaryKeyLong(),
                Name = "Patient" + x.GetPrimaryKeyLong()
            });
        }

        [HttpGet]
        [Route("messages/{id:long}/{patientId:long}")]
        public Task<IEnumerable<ChatMessage>> Messages(long id, long patientId)
        {
            var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(id);
            return provider.Messages(patientId);
        }

        [HttpPost]
        [Route("messages")]
        public async Task<IHttpActionResult> SendMessage(ProviderChatMessage message)
        {
            var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(message.Id);
            await provider.SendMessage(
                message.PatientId,
                new ChatMessage
                {
                    Name = "Provider" + message.Id,
                    Text = message.Message,
                    Received = DateTime.UtcNow
                }
            );

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}