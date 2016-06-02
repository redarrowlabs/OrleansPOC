using AppSettingsByConvention;
using Client.Infrastructure;
using Client.ViewModels;
using Common;
using Flurl;
using Flurl.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var config = SettingsByConvention.ForInterface<IConfiguration>();

            var cp = User as ClaimsPrincipal;
            var userId = Guid.Parse(cp.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var token = cp.Claims.First(x => x.Type == "token").Value;
            var model = new ChatViewModel
            {
                ApiBaseUrl = config.ApiBaseUrl,
                Token = token
            };

            var roles = cp.Claims.Where(x => x.Type == "role");
            if (roles.Any(x => x.Value == "patient"))
            {
                var patient = await config.ApiBaseUrl
                    .AppendPathSegments("api", "patient", userId, "chat", "name")
                    .WithHeader("Authorization", $"Bearer {token}")
                    .GetJsonAsync<Patient>();

                model.Id = userId;
                model.Name = patient.Name;

                return View("PatientChat", model);
            }
            else if (roles.Any(x => x.Value == "provider"))
            {
                var provider = await config.ApiBaseUrl
                    .AppendPathSegments("api", "provider", userId, "chat", "name")
                    .WithHeader("Authorization", $"Bearer {token}")
                    .GetJsonAsync<Provider>();

                model.Id = userId;
                model.Name = provider.Name;

                return View("ProviderChat", model);
            }

            return View();
        }
    }
}