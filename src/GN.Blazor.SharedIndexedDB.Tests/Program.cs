using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB;
using Fluxor;
using Fluxor.DependencyInjection;

namespace GN.Blazor.SharedIndexedDB.Tests
{
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
