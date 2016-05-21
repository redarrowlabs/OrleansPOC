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
    public class ProviderController : Controller
    {
        public async Task<ActionResult> Chat(long id)
        {
            var config = SettingsByConvention.ForInterface<IConfiguration>();

            var provider = await config.ApiBaseUrl
                .AppendPathSegments("provider", id.ToString(), "chat", "name")
                .GetJsonAsync<Provider>();

            var model = new ChatViewModel
            {
                Id = provider.Id,
                Name = provider.Name,
                ApiBaseUrl = config.ApiBaseUrl
            };

            return View(model);
        }
    }
}