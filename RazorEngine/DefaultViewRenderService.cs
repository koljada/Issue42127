using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace RazorEngine;

public abstract class DefaultViewRenderService : IViewRenderService
{
    private readonly IRazorViewEngine _razorEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public DefaultViewRenderService(
        IRazorViewEngine razorViewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _razorEngine = razorViewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    protected abstract RouteData RegisterRouteData(RouteData routeData = null, RouteValueDictionary routeValueDictionary = null);

    public async Task<string> RenderAsString<T>(string viewPath, T viewModel)
    {
        if (string.IsNullOrEmpty(viewPath))
            throw new ArgumentNullException($"View path can not be empty or null.");

        var actionContext = GetActionContext();

        using (var writer = new StringWriter())
        {
            var view = FindView(actionContext, viewPath);

            if (view == null)
                throw new InvalidOperationException($"The view {viewPath} wasn't found. due to invalid view path");

            var viewDictionary = new ViewDataDictionary<T>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = viewModel
            };

            var viewContext = new ViewContext(
                actionContext,
                view,
                viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                writer,
                new HtmlHelperOptions())
            {
                RouteData = RegisterRouteData() ?? DefaultDataRoute.GetValue()
            };

            await view.RenderAsync(viewContext);

            return writer.ToString();
        }
    }

    private IView FindView(ActionContext actionContext, string viewName)
    {
        var getViewResult = _razorEngine.GetView(viewName, viewName, isMainPage: true);
        if (getViewResult.Success)
            return getViewResult.View;

        var findViewResult = _razorEngine.FindView(actionContext, viewName, false);
        if (findViewResult.Success)
            return findViewResult.View;

        var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
        var errorMessage = string.Join(
            Environment.NewLine,
            new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations)); ;

        throw new InvalidOperationException(errorMessage);
    }

    private ActionContext GetActionContext()
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = _serviceProvider
        };
        var app = new ApplicationBuilder(_serviceProvider);
        var routeBuilder = new RouteBuilder(app)
        {
            DefaultHandler = new CustomRouter()
        };

        routeBuilder.MapRoute(
            string.Empty,
            "{controller}/{action}/{id}",
            new RouteValueDictionary(new { id = "defaultid" }));

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        actionContext.RouteData.Routers.Add(routeBuilder.Build());
        return actionContext;
    }

    internal class CustomRouter : IRouter
    {
        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            return Task.CompletedTask;
        }
    }
}