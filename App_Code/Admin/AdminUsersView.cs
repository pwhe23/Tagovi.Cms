using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Site
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersView : ViewModelBase
    {
        private readonly SiteDb _db;

        public AdminUsersView(SiteDb db)
        {
            _db = db;
        }

        public bool OnlyAdmin { get; set; }
        public int? Id { get; set; }
        public string NewPassword { get; set; }
        public User User { get; set; }

        public override void Initialize(Controller controller, string viewName)
        {
            User = _db.Users.Find(Id) ?? new User();
            base.Initialize(controller, viewName);
        }

        public List<User> QueryUsers()
        {
            var query = _db.Users.AsQueryable();
            if (OnlyAdmin)
            {
                query = query.Where(x => x.IsAdmin);
            }
            return query.ToList();
        }

        public ActionResult SaveUser()
        {
            if (NewPassword.HasValue()) User.Password = NewPassword;
            if (!Id.HasValue()) _db.Users.Add(User);
            _db.SaveChanges();
            return Redirect("/Admin/Users");
        }

        public ActionResult DeleteUser()
        {
            if (Id.HasValue()) _db.Users.Remove(User);
            _db.SaveChanges();
            return Redirect("/Admin/Users");
        }
    };
}
