using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WSB.Activity.Mvc.Filters
{
    public class ForbiddenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            throw new Exception("禁止访问!");
        }
    }
}