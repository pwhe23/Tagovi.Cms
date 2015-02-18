using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Newtonsoft.Json;

namespace Site
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString Section(this HtmlHelper html, string section)
        {
            var sb = new StringBuilder();
            using (var db = new SiteDb())
            {
                foreach (var part in db.Parts.Where(x => x.Parent == null && x.Section == section))
                {
                    var data = new ViewDataDictionary(part);
                    sb.AppendLine(html.Partial("~/content/parts/" + part.Type + ".cshtml", data).ToHtmlString());
                }
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        //REF: http://stackoverflow.com/a/4360017/366559
        public static MvcHtmlString Concat(this MvcHtmlString str, params MvcHtmlString[] items)
        {
            var sb = new StringBuilder();
            if (str != null)
            {
                sb.Append(str);
            }

            foreach (var item in items.Where(i => i != null))
            {
                sb.Append(item.ToHtmlString());
            }

            return MvcHtmlString.Create(sb.ToString());
        }
    };
}
