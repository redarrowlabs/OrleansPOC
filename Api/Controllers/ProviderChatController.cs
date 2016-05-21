using Api.Models;
using Common;
using GrainInterfaces;
using Orleans;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Controllers
{
    [RoutePrefix("provider/{id:long}/chat")]
    public class ProviderChatController : ApiController
    {
        [HttpGet]
        [Route("name")]
        public async Task<Provider> Name(long id)
        {
            var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(id);

            return new Provider
            {
                Id = id,
                Name = await provider.GetName()
            };
        }

        [HttpGet]
        [Route("patients")]
        public async Task<IEnumerable<Patient>> Patients(long id)
        {
            var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(id);

            return await provider.CurrentPatients();
        }

        [HttpGet]
        [Route("messages/{patientId:long}")]
        public Task<IEnumerable<ChatMessage>> Messages(long id, long patientId)
        {
            var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(id);

            return provider.Messages(patientId);
        }

        [HttpPost]
        [Route("messages")]
        public async Task<IHttpActionResult> SendMessage(long id, ProviderChatRequest request)
        {
            var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(id);
            await provider.SendMessage(request.PatientId, request.Text);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}