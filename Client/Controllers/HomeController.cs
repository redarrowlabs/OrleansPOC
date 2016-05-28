using AppSettingsByConvention;
using Client.Infrastructure;
using Client.ViewModels;
using Common;
using Flurl;
using Flurl.Http;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var config = SettingsByConvention.ForInterface<IConfiguration>();
            var token = ((ClaimsPrincipal)User).Claims.First(x => x.Type == "token").Value;
            var model = new ChatViewModel
            {
                ApiBaseUrl = config.ApiBaseUrl,
                Token = token
            };

            var roles = ((ClaimsPrincipal)User).Claims.Where(x => x.Type == "role");
            if (roles.Any(x => x.Value == "patient"))
            {
                var patient = await config.ApiBaseUrl
                    .AppendPathSegments("api", "patient", 1, "chat", "name")
                    .WithHeader("Authorization", $"Bearer {token}")
                    .GetJsonAsync<Patient>();

                model.Id = 1;
                model.Name = patient.Name;

                return View("PatientChat", model);
            }
            else if (roles.Any(x => x.Value == "provider"))
            {
                var provider = await config.ApiBaseUrl
                    .AppendPathSegments("api", "provider", 1, "chat", "name")
                    .WithHeader("Authorization", $"Bearer {token}")
                    .GetJsonAsync<Provider>();

                model.Id = 1;
                model.Name = provider.Name;

                return View("ProviderChat", model);
            }

            return View();
        }
    }
}