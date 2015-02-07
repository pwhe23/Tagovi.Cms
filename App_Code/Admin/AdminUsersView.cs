using System.Collections.Generic;
using System.Linq;

namespace Site
{
    public class AdminUsersView
    {
        private readonly SiteDb _db;

        public AdminUsersView(SiteDb db)
        {
            _db = db;
        }

        public List<User> GetUsers()
        {
            return _db.Users.ToList();
        }
    };
}
