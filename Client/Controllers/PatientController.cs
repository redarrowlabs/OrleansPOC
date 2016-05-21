using AppSettingsByConvention;
using Client.Infrastructure;
using Client.ViewModels;
using Common;
using Flurl;
using Flurl.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Client.Controllers
{
    public class PatientController : Controller
    {
        public async Task<ActionResult> Chat(long id)
        {
            var config = SettingsByConvention.ForInterface<IConfiguration>();

            var patient = await config.ApiBaseUrl
                .AppendPathSegments("patient", id.ToString(), "chat", "name")
                .GetJsonAsync<Patient>();

            var model = new ChatViewModel
            {
                Id = patient.Id,
                Name = patient.Name,
                ApiBaseUrl = config.ApiBaseUrl
            };

            return View(model);
        }
    }
}