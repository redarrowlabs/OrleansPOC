using Client.ViewModels;
using Common;
using Flurl;
using Flurl.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Client.Controllers
{
    public class ProviderController : Controller
    {
        private const string API_BASE_URL = "http://localhost:8090";

        public async Task<ActionResult> Chat(long id)
        {
            var provider = await API_BASE_URL
                .AppendPathSegments("provider", id.ToString(), "chat", "name")
                .GetJsonAsync<Provider>();

            var model = new ChatViewModel
            {
                Id = provider.Id,
                Name = provider.Name,
                ApiBaseUrl = API_BASE_URL
            };

            return View(model);
        }
    }
}