using Client.Models;
using Client.ViewModels;
using Flurl;
using Flurl.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Client.Controllers
{
    public class ProviderController : Controller
    {
        private const string BASE_URI = "http://localhost:8090";

        public ActionResult Chat(long id)
        {
            var model = new ChatViewModel
            {
                Id = id,
                Name = "Provider " + id
            };

            return View(model);
        }

        public async Task<ActionResult> Patients(long id)
        {
            var patients = await BASE_URI
                .AppendPathSegments("provider", "chat", "patients", id.ToString())
                .GetJsonAsync<IEnumerable<PatientOption>>();

            return Json(patients, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Messages(long id, long patientId)
        {
            var messages = await BASE_URI
                .AppendPathSegments("provider", "chat", "messages", id.ToString(), patientId.ToString())
                .GetJsonAsync<IEnumerable<UserChatMessage>>();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Message(long id, long patientId, string message)
        {
            await BASE_URI
                .AppendPathSegments("provider", "chat", "messages")
                .PostJsonAsync(new
                {
                    id = id,
                    patientId = patientId,
                    text = message
                });

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}