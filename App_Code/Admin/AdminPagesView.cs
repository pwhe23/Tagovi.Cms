using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Site
{
    [Authorize(Roles = "Admin")]
    public class AdminPagesView : ViewModelBase
    {
        private readonly SiteDb _db;

        public AdminPagesView(SiteDb db)
        {
            _db = db;
        }

        public int? Id { get; set; }
        public Page Page { get; set; }

        public override void Initialize(Controller controller, string viewName)
        {
            Page = _db.Pages.Find(Id) ?? new Page();
            base.Initialize(controller, viewName);
        }

        public List<Page> QueryPages()
        {
            var query = _db.Pages
                           .AsQueryable();

            return query.ToList();
        }

        public ActionResult SavePage()
        {
            if (!Id.HasValue()) _db.Pages.Add(Page);
            _db.SaveChanges();

            return Redirect("/Admin/Pages");
        }

        public ActionResult DeletePage()
        {
            if (Id.HasValue()) _db.Pages.Remove(Page);
            _db.SaveChanges();

            return Redirect("/Admin/Pages");
        }
    };
}
