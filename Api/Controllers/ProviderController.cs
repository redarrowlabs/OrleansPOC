using Common;
using GrainInterfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Controllers
{
    [RoutePrefix("provider/{id:Guid}")]
    public class ProviderController : ApiController
    {
        [HttpGet]
        [Route("name")]
        public async Task<Provider> Name(Guid id)
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
        public async Task<IEnumerable<Patient>> Patients(Guid id)
        {
            var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(id);

            return await provider.CurrentPatients();
        }
    }
}