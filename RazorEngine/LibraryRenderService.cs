using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace RazorEngine;

public class LibraryRenderService : DefaultViewRenderService
{
    public LibraryRenderService(
       IRazorViewEngine razorViewEngine,
       ITempDataProvider tempDataProvider,
       IServiceProvider serviceProvider)
        : base(razorViewEngine, tempDataProvider, serviceProvider)
    { }

    protected override RouteData RegisterRouteData(RouteData routeData = null, RouteValueDictionary routeValueDictionary = null)
    {
        if (routeData == null && routeValueDictionary == null)
            return DefaultDataRoute.GetValue();

        RouteData configuredRouteData;
        if (routeData != null)
            configuredRouteData = new RouteData(routeData);
        else
            configuredRouteData = new RouteData(routeValueDictionary);

        return configuredRouteData;
    }
}
