using System.Web;
using System.Web.Mvc;
using WSB.Activity.Mvc.Filters;

namespace WSB.Activity.Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionAttribute());
        }
    }
}
