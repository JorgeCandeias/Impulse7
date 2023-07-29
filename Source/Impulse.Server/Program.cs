using Impulse.Grains;
using Impulse.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orleans.Configuration;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

var ports = new
{
    Primary = 11111,
    Silo = TcpUtility.GetAvailablePort(11111, 11999),
    Gateway = TcpUtility.GetAvailablePort(30000, 30999),
    Dashboard = TcpUtility.GetAvailablePort(40000, 40999)
};

var builder = Host.CreateApplicationBuilder();

// hardcode the target enrivonment for sample purposes
builder.Environment.EnvironmentName = Environments.Development;

// add appropriate loggers per environment
builder.Logging.ClearProviders();
if (builder.Environment.IsDevelopment())
{
    // the console logger must only run in development due its performance impact
    builder.Logging.AddSerilog(new LoggerConfiguration()
        .MinimumLevel.Information()
        .Filter.ByExcluding(x => x.Properties.TryGetValue("SourceContext", out var property) && property is ScalarValue value && value.Value is string str && str == "Orleans.Runtime.SiloControl" && x.Level <= LogEventLevel.Information)
        .Filter.ByExcluding(x => x.Properties.TryGetValue("SourceContext", out var property) && property is ScalarValue value && value.Value is string str && str == "Orleans.Runtime.Management.ManagementGrain" && x.Level <= LogEventLevel.Information)
        .WriteTo.Console()
        .CreateLogger());
}
else
{
    // add production loggers
}

// add orleans services for any environment
builder.UseOrleans(orleans =>
{
    orleans
        .AddActivityGrainCallFilter(options =>
        {
            options.AllowedAssemblies.Add(typeof(ActivityGrainCallFilterOptions).Assembly);
        })
        .AddActivityPropagation()
        .AddMemoryStreams("Chat")
        .UseDashboard(options =>
        {
            options.Port = ports.Dashboard;
        });

    // add dashboard cpu and memory statistics
    // this is old code for v3 - it does not yet have a replacement for v7
    /*
    if (OperatingSystem.IsWindows())
    {
        builder.AddPerfCountersTelemetryConsumer();
    }
    else if (OperatingSystem.IsLinux())
    {
        builder.UseLinuxEnvironmentStatistics();
    }
    */
});

// add orleans services as appropriate for each environment
if (builder.Environment.IsDevelopment())
{
    builder.UseOrleans(orleans =>
    {
        orleans
            .UseLocalhostClustering(ports.Silo, ports.Gateway, new IPEndPoint(IPAddress.Loopback, ports.Primary))
            .AddMemoryGrainStorageAsDefault()
            .AddMemoryGrainStorage("PubSubStore");
    });
}
else
{
    builder.UseOrleans(orleans =>
    {
        orleans
            .ConfigureEndpoints(ports.Silo, ports.Gateway, AddressFamily.InterNetwork, true)
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = nameof(Impulse);
                options.ServiceId = nameof(Impulse);
            })
            .UseAdoNetClustering(options =>
            {
                options.Invariant = "Microsoft.Data.SqlClient";
                options.ConnectionString = builder.Configuration.GetConnectionString("Orleans");
            })
            .AddAdoNetGrainStorageAsDefault(options =>
            {
                options.Invariant = "Microsoft.Data.SqlClient";
                options.ConnectionString = builder.Configuration.GetConnectionString("Orleans");
            })
            .AddAdoNetGrainStorage("PubSubStore", options =>
            {
                options.Invariant = "Microsoft.Data.SqlClient";
                options.ConnectionString = builder.Configuration.GetConnectionString("Orleans");
            })
            .UseAdoNetReminderService(options =>
            {
                options.Invariant = "Microsoft.Data.SqlClient";
                options.ConnectionString = builder.Configuration.GetConnectionString("Orleans");
            });
    });
}

// add open telemetry services for all environments
builder.Services.AddSingleton(sp => new ActivitySource(nameof(Impulse)));

// add open telemetry as appropriate for each environment
if (builder.Environment.IsDevelopment())
{
    builder.Services
        .AddOpenTelemetry()
        .WithTracing(options =>
        {
            options
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(nameof(Impulse)))
                .AddSource(nameof(Impulse))
                .AddConsoleExporter(x =>
                {
                    x.Targets = ConsoleExporterOutputTargets.Console | ConsoleExporterOutputTargets.Debug;
                });
        });
}
else
{
    builder.Services
        .AddOpenTelemetry();
}

using var host = builder.Build();

await host.StartAsync();

if (Environment.UserInteractive)
{
    Console.Title = $"Silo: {ports.Silo}, Gateway: {ports.Gateway}, Dashboard: {ports.Dashboard}";
}

await host.WaitForShutdownAsync();