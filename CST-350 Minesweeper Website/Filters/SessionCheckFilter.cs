using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CST_350_Minesweeper_Website.Filters
{
    public class SessionCheckFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Session.GetString("User") == null)
            {
                context.Result = new RedirectResult("/Login/Index");
            }
        }
    }
}
