using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Site
{
    public class AdminUsersView : ViewModelBase
    {
        private readonly SiteDb _db;

        public AdminUsersView(SiteDb db)
        {
            _db = db;
        }

        public int? Id { get; set; }
        public string NewPassword { get; set; }
        public User User { get; set; }

        public override void Initialize(Controller controller, string viewName)
        {
            User = _db.Users.Find(Id) ?? new User();
            base.Initialize(controller, viewName);
        }

        public List<User> GetUsers()
        {
            return _db.Users.ToList();
        }

        public ActionResult Save()
        {
            if (NewPassword.HasValue()) User.Password = NewPassword;
            if (!Id.HasValue()) _db.Users.Add(User);
            _db.SaveChanges();
            return Redirect("/Admin/Users");
        }

        public ActionResult Delete()
        {
            if (Id.HasValue()) _db.Users.Remove(User);
            _db.SaveChanges();
            return Redirect("/Admin/Users");
        }
    };
}
