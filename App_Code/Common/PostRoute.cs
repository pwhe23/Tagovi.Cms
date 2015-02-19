using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Site
{
    public class PostRoute : RouteBase
    {
        private static readonly Dictionary<string, int?> _pages;

        static PostRoute()
        {
            _pages = LoadPageRoutes();
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var path = httpContext.Request.AppRelativeCurrentExecutionFilePath ?? "";
            var paths = path.ToLower().Split(new[] {'~', '/'}, StringSplitOptions.RemoveEmptyEntries);
            var parent = string.Join("/", paths.Leave(1));
            if (string.IsNullOrWhiteSpace(parent))
                parent = "";

            if (!_pages.ContainsKey(parent))
                return null;

            Post post;
            using (var db = new SiteDb())
            {
                var parentId = _pages[parent];
                var slug = paths.Last();
                post = db.Posts
                         .Include(x => x.User)
                         .SingleOrDefault(x => x.PageId == parentId && x.Slug == slug);
            }
            if (post == null)
                return null;

            // Return controller action
            var route = new RouteData(this, new MvcRouteHandler());
            route.Values.Add("controller", "Post");
            route.Values.Add("action", "ViewPost");
            route.Values.Add("post", post);

            return route;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }

        private static Dictionary<string, int?> LoadPageRoutes()
        {
            using (var db = new SiteDb())
            {
                return db.Pages.ToDictionary(x => x.Url == null ? "" : x.Url.ToLower(), x => x.Url == null ? null : (int?)x.Id);
            }
        }
    };

    public class PostController : Controller
    {
        public ActionResult ViewPost(Post post)
        {
            return View("~/content/posts/Post.cshtml", post);
        }
    };
}