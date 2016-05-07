using System.Web.Mvc;

namespace Client.Controllers
{
    public class PatientController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}