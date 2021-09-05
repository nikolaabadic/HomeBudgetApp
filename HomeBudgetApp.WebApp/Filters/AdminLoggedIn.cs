using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetApp.WebApp.Filters
{
    public class AdminLoggedIn : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Filters.OfType<AdminNotLoggedIn>().Any())
            {
                return;
            }

            if (context.HttpContext.Session.GetInt32("adminid") == null)
            {
                if (context.HttpContext.Session.GetInt32("userid") != null)
                {
                    context.HttpContext.Response.Redirect("/User/Details");
                    return;
                }
                context.HttpContext.Response.Redirect("/Admin/Options");
                return;
            }
            else
            {
                Controller controller = (Controller)context.Controller;
                controller.ViewBag.AdminIsLoggedIn = true;
            }
        }
    }
}
