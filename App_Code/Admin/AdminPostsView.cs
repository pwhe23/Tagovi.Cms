using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Site
{
    [Authorize(Roles = "Admin")]
    public class AdminPostsView : ViewModelBase
    {
        private readonly SiteDb _db;

        public AdminPostsView(SiteDb db)
        {
            _db = db;
        }

        public int? Id { get; set; }
        public Post Post { get; set; }

        public override void Initialize(Controller controller, string viewName)
        {
            Post = _db.Posts.Find(Id) ?? new Post();
            base.Initialize(controller, viewName);
        }

        public List<Post> QueryPosts()
        {
            var query = _db.Posts
                           .AsQueryable();

            return query.OrderByDescending(x => x.Created)
                        .ToList();
        }

        public ActionResult SavePost()
        {
            if (!Id.HasValue()) _db.Posts.Add(Post);
            _db.SaveChanges();

            return Redirect("/Admin/Posts");
        }

        public ActionResult DeletePost()
        {
            if (Id.HasValue()) _db.Posts.Remove(Post);
            _db.SaveChanges();

            return Redirect("/Admin/Posts");
        }
    };
}
