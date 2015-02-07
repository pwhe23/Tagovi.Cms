using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using SimpleInjector;

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

            //See if they passed an Id
            var arr = path.Split('/');
            var id = StrToInt(arr[arr.Length - 1]);
            if (id.HasValue)
            {
                path = string.Join("/", arr.Take(arr.Length - 1));
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
            if (id.HasValue)
            {
                route.Values.Add("id", id);
            }

            return route;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }

        private static int? StrToInt(string num)
        {
            if (string.IsNullOrEmpty(num))
                return null;

            int result;
            return int.TryParse(num, out result) ? result : (int?)null;
        }
    };

    public class ViewController : Controller
    {
        private static readonly Dictionary<string, Type> _models = new Dictionary<string, Type>();
        private readonly Container _container;

        public ViewController(Container container)
        {
            _container = container;
        }

        public ActionResult ViewPage(int? id, string view)
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
            var model = _container.GetInstance(modelType);
            if (id.HasValue)
            {
                model.GetType().GetProperty("Id").SetValue(model, id.Value);
            }

            //If Model is ViewModelBase let it do it's thing
            if (model is ViewModelBase)
            {
                return ExecuteViewModel(view, (ViewModelBase)model);
            }

            //Else return standard View handling
            TryUpdateModel(model, null, null, null, null);
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

        private ActionResult ExecuteViewModel(ViewModelBase viewModel)
        {
            return ExecuteViewModel(null, viewModel);
        }

        private ActionResult ExecuteViewModel(string viewName, ViewModelBase viewModel)
        {
            if (viewModel == null)
                return null;

            viewModel.Initialize(this, viewName);
            TryUpdateModel(viewModel, null, null, null, null);

            var submit = Request.Form["_submit"];
            if (string.IsNullOrWhiteSpace(submit))
                submit = "Load";

            return ExecuteViewModel(viewModel, submit);
        }

        private ActionResult ExecuteViewModel(ViewModelBase viewModel, string submit)
        {
            var method = viewModel.GetType().GetMethod(submit);

            try
            {
                var result = method.Invoke(viewModel, new object[0]);
                return (ActionResult)result;
            }
            catch (Exception ex)
            {
                if (submit == "Load")
                    throw;

                ViewData["Error"] = ex.InnerException.Message;
                return ExecuteViewModel(viewModel, "Load");
            }
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
            if (Request.HttpMethod != "POST") return false;
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

    public abstract class ViewModelBase
    {
        protected Controller Controller;
        protected string ViewName;

        public virtual void Initialize(Controller controller, string viewName)
        {
            Controller = controller;
            ViewName = viewName;
        }

        public virtual ActionResult Load()
        {
            return View();
        }

        protected ActionResult View(object model = null)
        {
            Controller.ViewData.Model = model ?? this;
            return new ViewResult
            {
                ViewData = Controller.ViewData,
                TempData = Controller.TempData,
                ViewName = ViewName,
                ViewEngineCollection = Controller.ViewEngineCollection,
            };
        }

        protected PartialViewResult PartialView(object model = null)
        {
            Controller.ViewData.Model = model ?? this;
            return new PartialViewResult
            {
                ViewData = Controller.ViewData,
                TempData = Controller.TempData,
                ViewName = ViewName,
                ViewEngineCollection = Controller.ViewEngineCollection,
            };
        }

        protected ActionResult Redirect(string url)
        {
            return new RedirectResult(url);
        }
    };
}
