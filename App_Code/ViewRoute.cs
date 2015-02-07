using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace Site
{
    /// <summary>
    /// Loads cshtml files under /Views/ without a specific controller action
    /// </summary>
    public class ViewRoute : RouteBase
    {
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var path = httpContext.Request.AppRelativeCurrentExecutionFilePath;

            //Default to Index
            if (path.EndsWith("/"))
            {
                path += "Index";
            }

            //See if the View exists
            var view = path.Replace("~/", "~/Views/") + ".cshtml";
            if (!File.Exists(HostingEnvironment.MapPath(view)))
            {
                return null;
            }

            // Return controller action
            var route = new RouteData(this, new MvcRouteHandler());
            route.Values.Add("controller", "View");
            route.Values.Add("action", "ViewPage");
            route.Values.Add("view", view);

            return route;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }
    };

    public class ViewController : Controller
    {
        private static readonly Dictionary<string, Type> _models = new Dictionary<string, Type>();

        public ActionResult ViewPage(string view)
        {
            //See if we've already cached the Model for this View
            if (!_models.ContainsKey(view))
            {
                _models[view] = GetTypeFromView(HostingEnvironment.MapPath(view));
            }

            //If the View doesn't have a Model we're done
            var modelType = _models[view];
            if (modelType == null)
            {
                return View(view);
            }

            //Create an instance of the View's Model and Update it
            var model = Activator.CreateInstance(modelType);
            TryUpdateModel(model, null, null, null, null);

            //Else return standard View handling
            return View(view, model);
        }

        [ChildActionOnly]
        public PartialViewResult ViewPartial(string view)
        {
            if (!view.StartsWith("~"))
            {
                view = "~/Views/Shared/" + view;
            }
            if (!view.EndsWith(".cshtml"))
            {
                view += ".cshtml";
            }

            //See if we've already cached the Model for this View
            if (!_models.ContainsKey(view))
            {
                _models[view] = GetTypeFromView(HostingEnvironment.MapPath(view));
            }

            //If the View doesn't have a Model we're done
            var modelType = _models[view];
            if (modelType == null)
            {
                return PartialView(view);
            }

            var model = Activator.CreateInstance(modelType);
            return PartialView(view, model);
        }

        private static Type GetTypeFromView(string path)
        {
            //We're going to assume the View's first line looks like this "@model Full.Class.Name"
            var lines = System.IO.File.ReadAllLines(path);
            if (lines.Length < 1)
            {
                return null;
            }

            var arr = lines[0].Trim().Split(' ');
            if (arr.Length != 2 || arr[0] != "@model" || arr[1] == "dynamic")
            {
                return null;
            }

            return Type.GetType(arr[1], true);
        }

        protected bool TryUpdateModel(object model, string prefix, string[] includeProperties, string[] excludeProperties, IValueProvider valueProvider)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (valueProvider == null) valueProvider = ValueProviderFactories.Factories.GetValueProvider(ControllerContext);
            var binders = ModelBinders.Binders;
            var binder = binders.GetBinder(model.GetType());

            var bindingContext = new ModelBindingContext();
            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());
            bindingContext.ModelName = prefix;
            bindingContext.ModelState = ViewData.ModelState;
            bindingContext.PropertyFilter = propertyName => IsPropertyAllowed(propertyName, includeProperties, excludeProperties);
            bindingContext.ValueProvider = valueProvider;
            binder.BindModel(ControllerContext, bindingContext);
            return ViewData.ModelState.IsValid;
        }

        internal static bool IsPropertyAllowed(string propertyName, string[] includeProperties, string[] excludeProperties)
        {
            var flag = ((includeProperties == null) || (includeProperties.Length == 0)) || includeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            var flag2 = (excludeProperties != null) && excludeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            return (flag && !flag2);
        }
    };
}
