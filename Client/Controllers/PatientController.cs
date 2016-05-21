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
        private const string API_BASE_URL = "http://localhost:8090";

        public async Task<ActionResult> Chat(long id)
        {
            var patient = await API_BASE_URL
                .AppendPathSegments("patient", id.ToString(), "chat", "name")
                .GetJsonAsync<Patient>();

            var model = new ChatViewModel
            {
                Id = patient.Id,
                Name = patient.Name,
                ApiBaseUrl = API_BASE_URL
            };

            return View(model);
        }
    }
}