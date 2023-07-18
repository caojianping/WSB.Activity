using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using WSB.Activity.Interface;

namespace WSB.Activity.Mvc.Filters
{
    public class RedpacketsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var suser = HttpContext.Current.Session["SUSER"];
            if (suser != null && suser is SUSER)
            {
                IRedpacketsService service = DIFactory.GetContainer().Resolve<IRedpacketsService>();
                service.SetInvalid(((SUSER)suser).UserId, Properties.Settings.Default.BigRedpackets_ExpireTime);
            }
        }
    }
}