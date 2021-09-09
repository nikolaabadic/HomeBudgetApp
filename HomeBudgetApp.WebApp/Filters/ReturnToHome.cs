using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetApp.WebApp.Filters
{
    public class ReturnToHome : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            if (context.HttpContext.Session.GetInt32("userid") != null)
            {
                context.HttpContext.Response.Redirect("/User/Details");
                return;
            }

            if (context.HttpContext.Session.GetInt32("adminid") != null)
            {
                context.HttpContext.Response.Redirect("/Admin/Options");
                return;
            }
        }
    }
}
