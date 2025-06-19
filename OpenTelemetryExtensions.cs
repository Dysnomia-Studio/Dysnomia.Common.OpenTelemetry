using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Dysnomia.Common.OpenTelemetry {
    public static class OpenTelemetryExtensions {
        public static void EnableOpenTelemetry(this IServiceCollection serviceCollection, IHostEnvironment environment) {
            var openTelemetryBuilder = serviceCollection.AddOpenTelemetry()
                .WithTracing(otBuilder => {

                    otBuilder
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(environment.ApplicationName));
                })
                .WithMetrics(otBuilder => {

                    otBuilder.AddMeter(
                        "Microsoft.AspNetCore.Hosting",
                        "Microsoft.AspNetCore.Routing",
                        "Microsoft.AspNetCore.Diagnostics",
                        "Microsoft.AspNetCore.RateLimiting",
                        "Microsoft.AspNetCore.HeaderParsing",
                        "Microsoft.AspNetCore.Server.Kestrel",
                        "Microsoft.AspNetCore.Http.Connections"
                    )
                    .AddHttpClientInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddSqlClientInstrumentation()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(environment.ApplicationName));
                })
                .WithLogging(otBuilder => {
                    otBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(environment.ApplicationName));

                    if (environment.EnvironmentName == "Development") {
                        otBuilder.AddConsoleExporter();
                    }
                });

            if (environment.EnvironmentName != "Development") {
                openTelemetryBuilder.UseOtlpExporter();
            }
        }

    }
}
