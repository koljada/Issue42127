namespace RazorEngine;

public interface IViewRenderService
{
    Task<string> RenderAsString<T>(string viewPath, T viewModel);
}