using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Site
{
    public class AdminLogoutView : ViewModelBase
    {
        public override ActionResult Load()
        {
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
    };
}
