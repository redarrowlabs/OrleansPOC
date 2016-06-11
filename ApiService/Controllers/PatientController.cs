using Common;
using GrainInterfaces;
using Orleans;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiService.Controllers
{
    [RoutePrefix("patient/{id:Guid}")]
    public class PatientController : ApiController
    {
        [HttpGet]
        [Route("name")]
        public async Task<Patient> Name(Guid id)
        {
            var patient = GrainClient.GrainFactory.GetGrain<IPatientGrain>(id);

            return new Patient
            {
                Id = id,
                Name = await patient.GetName()
            };
        }
    }
}