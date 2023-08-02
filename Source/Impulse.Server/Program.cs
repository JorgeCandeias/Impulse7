using Impulse.Data.InMemory;
using Impulse.Data.SqlServer;
using Impulse.Grains;
using Impulse.Server;

var ports = new
{
    Primary = 11111,
    Silo = TcpUtility.GetAvailablePort(11111, 11999),
    Gateway = TcpUtility.GetAvailablePort(30000, 30999),
    Dashboard = TcpUtility.GetAvailablePort(40000, 40999)
};

var builder = Host.CreateApplicationBuilder();

// link up the global appsettings file
builder.Configuration.AddJsonFile("appsettings.global.json", false);

// link up the environment to a configuration key
builder.Environment.EnvironmentName = builder.Configuration["Environment"]!;

// add orleans services for all environments
builder.UseOrleans(orleans =>
{
    orleans
        //.AddActivityPropagation()
        .AddLoggingGrainCallFilter(options =>
        {
            options.AllowedAssemblies.Add(typeof(LoggingGrainCallFilterOptions).Assembly);
        })
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

// add services for development
if (builder.Environment.IsDevelopment())
{
    // the console logger must only run in development due its performance impact
    builder.Logging.ClearProviders();

    // add serilog while exluding chatty sources
    builder.Logging.AddSerilog(new LoggerConfiguration()
        .MinimumLevel.Information()
        .Filter.ByExcluding(x => x.Properties.TryGetValue("SourceContext", out var property) && property is ScalarValue value && value.Value is string str && str == "Orleans.Runtime.SiloControl" && x.Level <= LogEventLevel.Information)
        .Filter.ByExcluding(x => x.Properties.TryGetValue("SourceContext", out var property) && property is ScalarValue value && value.Value is string str && str == "Orleans.Runtime.Management.ManagementGrain" && x.Level <= LogEventLevel.Information)
        .WriteTo.Console()
        .CreateLogger());

    // add orleans with in-memory services for development
    builder.UseOrleans(orleans =>
    {
        orleans
            .UseLocalhostClustering(ports.Silo, ports.Gateway, new IPEndPoint(IPAddress.Loopback, ports.Primary))
            .AddMemoryGrainStorageAsDefault()
            .AddMemoryGrainStorage("PubSubStore")
            .UseInMemoryReminderService();
    });

    // add in-memory repositories for development
    builder.Services.AddInMemoryRepositories();

    // add telemetry exporters for development
    builder.Services
        .AddOpenTelemetry()
        .WithTracing(options =>
        {
            options
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(nameof(Impulse)))
                //.AddSource("Impulse")
                //.AddSource("Microsoft.Orleans.Runtime")
                //.AddSource("Microsoft.Orleans.Application")
                .AddConsoleExporter();
        });
}

// add services for production
else
{
    // add production loggers
    builder.Logging.ClearProviders();

    // add orleans with sql services for production
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

    // add sql repositories for production
    builder.Services.AddSqlRepositories(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("Application")!;
    });

    // add telemetry for production
    builder.Services.AddOpenTelemetry();
}

using var host = builder.Build();

await host.StartAsync();

if (Environment.UserInteractive)
{
    Console.Title = $"Silo: {ports.Silo}, Gateway: {ports.Gateway}, Dashboard: {ports.Dashboard}";
}

await host.WaitForShutdownAsync();