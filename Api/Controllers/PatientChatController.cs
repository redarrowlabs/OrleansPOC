using Common;
using GrainInterfaces;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Controllers
{
    [RoutePrefix("patient/{id:long}/chat")]
    public class PatientChatController : ApiController
    {
        [HttpGet]
        [Route("name")]
        public async Task<Patient> Name(long id)
        {
            var patient = GrainClient.GrainFactory.GetGrain<IPatientGrain>(id);

            return new Patient
            {
                Id = id,
                Name = await patient.GetName()
            };
        }

        [HttpGet]
        [Route("messages")]
        public Task<IEnumerable<ChatMessage>> Messages(long id)
        {
            var patient = GrainClient.GrainFactory.GetGrain<IPatientGrain>(id);

            return patient.Messages();
        }
    }
}