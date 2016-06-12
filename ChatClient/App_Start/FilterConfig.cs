using ChatClient.Infrastructure;
using System.Web.Mvc;

namespace ChatClient
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