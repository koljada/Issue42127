using Emails;
using Issue42127;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using RazorEngine;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Issue42127;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder) {

        builder.Services
            .AddLibraryViewRenderService<Helper>("Issue42127");
    }
}
