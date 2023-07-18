using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WSB.Activity.Mvc.Filters
{
    public class SessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var suser = HttpContext.Current.Session["SUSER"];
            if (suser == null || !(suser is SUSER))
            {
                throw new Exception("Session已经失效!");
            }
        }
    }
}