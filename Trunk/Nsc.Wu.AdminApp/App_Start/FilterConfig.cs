using System.Web;
using System.Web.Mvc;

namespace Nsc.Wu.AdminApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
