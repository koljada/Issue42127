using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RazorEngine;
using System;
using Models;
using System.Threading.Tasks;

namespace Issue42127;

public class Function1
{
    private readonly IViewRenderService _viewRenderService;

    public Function1(IViewRenderService viewRenderService)
    {
        _viewRenderService = viewRenderService;
    }

    [FunctionName("Function1")]
    public async Task Run([TimerTrigger("0 0 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
    {
        var okModel = new LicenceCountChanged(42, "test@test.com");
        var exceptionModel = new LicenceCountChangedWithException(42, "test@test.com");

        var html1 = await _viewRenderService.RenderAsString("EmailTemplates/LicenceCountChanged", okModel);
        log.LogInformation(html1);

        var html2 = await _viewRenderService.RenderAsString("EmailTemplates/LicenceCountChangedWithoutModel", exceptionModel);
        log.LogInformation(html2);

        try
        {
            await _viewRenderService.RenderAsString("EmailTemplates/LicenceCountChangedWithException", exceptionModel);
        }
        catch (InvalidOperationException ex)
        {
            log.LogError(ex, "Here it is");
        }
    }
}
