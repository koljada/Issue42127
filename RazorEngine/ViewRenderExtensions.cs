using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using System.Diagnostics;
using System.Reflection;

namespace RazorEngine;

public static class ViewRenderExtensions
{
    public static IServiceCollection AddLibraryViewRenderService<THtmlContentHelper>(this IServiceCollection services, string appName, string rootPath = null) where THtmlContentHelper : class
    {
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton<DiagnosticSource>(new DiagnosticListener("Microsoft.AspNetCore"));

        rootPath ??= Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        var emailsAssembly = typeof(THtmlContentHelper).Assembly;

        services.AddMvcCore()
            .AddViewLocalization()
            .AddViews()
            .AddRazorViewEngine()
            .AddApplicationPart(GetViewsAssembly(rootPath, appName))
            .AddApplicationPart(GetViewsAssembly(Path.GetDirectoryName(emailsAssembly.Location), emailsAssembly.GetName().Name));
        services
            .AddSingleton<THtmlContentHelper>()
            .AddScoped<IViewRenderService, LibraryRenderService>();

        return services;
    }
    private static Assembly GetViewsAssembly(string executionPath, string name)
    {
        var compiledViewsAssembly = Assembly.LoadFile(Path.Combine(executionPath, name + ".dll"));
        return compiledViewsAssembly;
    }
}
