using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Dysnomia.Common.OpenTelemetry {
	public static class OpenTelemetryExtensions {
		public static void EnableOpenTelemetry(IServiceCollection serviceCollection, IHostEnvironment environment) {
			var openTelemetryBuilder = serviceCollection.AddOpenTelemetry()
				.WithTracing(otBuilder => {

					otBuilder.AddAspNetCoreInstrumentation()
					.AddEntityFrameworkCoreInstrumentation()
					.AddHttpClientInstrumentation()
					.AddSqlClientInstrumentation()
					.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(environment.ApplicationName));
				})
				.WithMetrics(otBuilder => {
					otBuilder.AddAspNetCoreInstrumentation()
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
