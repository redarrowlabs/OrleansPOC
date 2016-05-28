using Client.Infrastructure;
using System.Web.Mvc;

namespace Client
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new JsonNetAttribute());
            filters.Add(new AuthorizeAttribute());
        }
    }
}