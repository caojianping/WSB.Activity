using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WSB.Activity.Common;

namespace WSB.Activity.Mvc.Filters
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ExceptionAttribute : HandleErrorAttribute
    {
        private Logger _logger = Logger.CreateLogger(typeof(WeChatAttribute));

        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                string controllerName = (string)filterContext.RouteData.Values["controller"];
                string actionName = (string)filterContext.RouteData.Values["action"];
                string msgTemplate = "执行controller[{0}]的action[{1}]产生异常";

                //记录日志
                _logger.Error(string.Format(msgTemplate, controllerName, actionName), filterContext.Exception);

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult()
                    {
                        Data = new AjaxResult()
                        {
                            Result = DoResult.Failed,
                            PromptMsg = "系统出现异常，请联系管理员！",
                            DebugMessage = filterContext.Exception.Message
                        }
                    };
                }
                else
                {
                    filterContext.Result = new ViewResult()
                    {
                        ViewName = "~/Views/Shared/Error.cshtml",
                        ViewData = new ViewDataDictionary<string>(filterContext.Exception.Message)
                    };
                }
                filterContext.ExceptionHandled = true;
            }
        }
    }
}