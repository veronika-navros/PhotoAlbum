using System.Web.Mvc;
using PhotoAlbum.WEB.Filters;

namespace PhotoAlbum.WEB
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionLoggerAttribute());
        }
    }
}
