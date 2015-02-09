using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Site
{
    public class AdminLoginView : ViewModelBase
    {
        private readonly SiteDb _db;

        public AdminLoginView(SiteDb db)
        {
            _db = db;
        }

        public string Email { get; set; }
        public string Password { get; set; }

        public ActionResult Login()
        {
            var user = _db.Users.SingleOrDefault(x => x.Email == Email);
            if (user == null || string.IsNullOrWhiteSpace(Password) || !user.Password.Equals(Password))
                throw new ApplicationException("Invalid Login");

            SetPrincipal(user.Email, new string[0]);
            return Redirect("/");
        }

        private static void SetPrincipal(string username, string[] roles)
        {
            var authTicket = new FormsAuthenticationTicket(
                1,
                username,
                DateTime.Now,
                DateTime.Now.AddMinutes(20),  // expiry
                true,  //do not remember
                String.Join(",", roles),
                "/");

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    };
}
