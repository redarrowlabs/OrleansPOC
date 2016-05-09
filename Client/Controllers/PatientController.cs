using Client.ViewModels;
using Common;
using Flurl;
using Flurl.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Client.Controllers
{
    public class PatientController : Controller
    {
        private const string BASE_URI = "http://localhost:8090";

        public ActionResult Chat(long id)
        {
            var model = new ChatViewModel
            {
                Id = id,
                Name = "Patient" + id
            };

            return View(model);
        }

        public async Task<ActionResult> Messages(long id)
        {
            var messages = await BASE_URI
                .AppendPathSegments("patient", "chat", "messages", id.ToString())
                .GetJsonAsync<IEnumerable<ChatMessage>>();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Message(long id, string message)
        {
            await BASE_URI
                .AppendPathSegments("patient", "chat", "messages")
                .PostJsonAsync(new
                {
                    id = id,
                    message = message
                });

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}