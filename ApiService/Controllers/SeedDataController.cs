using GrainInterfaces;
using Orleans;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiService.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("setup")]
    public class SeedDataController : ApiController
    {
        [HttpPost]
        [Route("data")]
        public async Task<IHttpActionResult> Index()
        {
            var pt1 = GrainClient.GrainFactory.GetGrain<IPatientGrain>(Guid.Parse("00000000-0000-0000-0000-000000000001"));
            var pt2 = GrainClient.GrainFactory.GetGrain<IPatientGrain>(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            var pr1 = GrainClient.GrainFactory.GetGrain<IProviderGrain>(Guid.Parse("00000000-0000-0000-0000-000000000003"));
            var pr2 = GrainClient.GrainFactory.GetGrain<IProviderGrain>(Guid.Parse("00000000-0000-0000-0000-000000000004"));

            await pt1.SetName("Justin Case");
            await pt2.SetName("Gene Poole");
            await pr1.SetName("Doctor Snuggles");
            await pr2.SetName("Doctor Nobody");
            await pr1.AddPatient(pt1);
            await pr1.AddPatient(pt2);

            return Ok();
        }
    }
}