using System.Web.Mvc;

namespace Client.Infrastructure
{
    public class JsonNetAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result.GetType() == typeof(JsonResult))
            {
                var result = filterContext.Result as JsonResult;
                filterContext.Result = new JsonNetResult
                {
                    ContentEncoding = result.ContentEncoding,
                    ContentType = result.ContentType,
                    Data = result.Data,
                    JsonRequestBehavior = result.JsonRequestBehavior
                };
            }

            base.OnActionExecuted(filterContext);
        }
    }
}