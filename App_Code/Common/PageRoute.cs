using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Site
{
    public class PageRoute : RouteBase
    {
        private static readonly Dictionary<string, int> _pages;

        static PageRoute()
        {
            _pages = LoadPageRoutes();
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var path = (httpContext.Request.AppRelativeCurrentExecutionFilePath ?? "").Replace("~/", "");
            if (!_pages.ContainsKey(path))
                return null;

            Page page;
            using (var db = new SiteDb())
            {
                var pageId = _pages[path];
                page = db.Pages.Single(x => x.Id == pageId);
            }

            // Return controller action
            var route = new RouteData(this, new MvcRouteHandler());
            route.Values.Add("controller", "Page");
            route.Values.Add("action", "ViewPage");
            route.Values.Add("page", page);

            return route;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }

        private static Dictionary<string, int> LoadPageRoutes()
        {
            using (var db = new SiteDb())
            {
                return db.Pages.ToDictionary(x => x.Url == null ? "" : x.Url.ToLower(), x => x.Id);
            }
        }
    };

    public class PageController : Controller
    {
        public ActionResult ViewPage(Page page)
        {
            return View("~/content/pages/Page.cshtml", page);
        }
    };
}