<%@ Application Language="C#" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="SimpleInjector" %>
<%@ Import Namespace="SimpleInjector.Integration.Web.Mvc" %>
<%@ Import Namespace="Site" %>

<script RunAt="server">
    public static Container Container { get; set; }
    void Application_Start(Object sender, EventArgs args)
    {
        Container = new Container();
        ConfigureContainer(Container);
        ConfigureRoutes(RouteTable.Routes);
    }

    void Session_Start(Object sender, EventArgs args)
    {
    }

    void Application_BeginRequest(Object sender, EventArgs args)
    {
    }

    void Application_Error(Object sender, EventArgs args)
    {
    }

    void Session_End(Object sender, EventArgs args)
    {
    }

    void Application_End(Object sender, EventArgs args)
    {
    }

    private static void ConfigureContainer(Container container)
    {
        container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
        container.RegisterMvcIntegratedFilterProvider();
        container.Verify();
        DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
    }

    private static void ConfigureRoutes(RouteCollection routes)
    {
        routes.Add("ViewRoute", new ViewRoute());
    }
</script>
