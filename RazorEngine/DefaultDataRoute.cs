using Microsoft.AspNetCore.Routing;

namespace RazorEngine;

public static class DefaultDataRoute
{
    public static RouteData GetValue()
    {
        var routeData = new RouteData();
        var routeValueDictionary = new RouteValueDictionary
        {
            { "action", "Default" },
            { "controller", "Default" }
        };

        routeData.PushState(new RouteCollection(), routeValueDictionary, null);

        return routeData;
    }

    public static RouteData GetValue(RouteValueDictionary routeValueDictionary)
    {
        var routeData = new RouteData();
        routeData.PushState(new RouteCollection(), routeValueDictionary, null);

        return routeData;
    }
}
