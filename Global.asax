<%@ Application Language="C#" %>
<%@ Import Namespace="Site" %>
<script RunAt="server">
    void Application_Start(Object sender, EventArgs args)
    {
        System.Web.Routing.RouteTable.Routes.Add("ViewRoute", new ViewRoute());
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
</script>
