namespace GN.Blazor.SharedIndexedDB.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fluxor;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddShilaFeatures();
            builder.Services.AddFluxor(cfg => cfg.ScanAssemblies(
                typeof(ShilaFeaturesExtensions).Assembly,
                typeof(Program).Assembly));

            await builder.Build().RunAsync();
        }
    }
}
