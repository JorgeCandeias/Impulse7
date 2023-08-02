using Impulse.Data.InMemory;
using Impulse.Data.SqlServer;
using Impulse.WebApp.Data;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orleans.Configuration;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// link up the global appsettings file
builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.global.json"), false);

// link up the environment to a configuration key
builder.Environment.EnvironmentName = builder.Configuration["Environment"]!;

// add services for all environments
builder.Services.AddOrleansClient(orleans =>
{
    orleans.AddMemoryStreams("Chat");
    //orleans.AddActivityPropagation();
});

// add open telemetry source for the app
builder.Services.AddSingleton(sp => new ActivitySource("Impulse"));

// add development services
if (builder.Environment.IsDevelopment())
{
    builder.Services
        .AddOrleansClient(orleans =>
        {
            orleans
                .UseLocalhostClustering();
        })
        .AddInMemoryRepositories();

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

// add production services
else
{
    builder.Services
        .AddOrleansClient(orleans =>
        {
            orleans
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = "Microsoft.Data.SqlClient";
                    options.ConnectionString = builder.Configuration.GetConnectionString("Orleans");
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = nameof(Impulse);
                    options.ServiceId = nameof(Impulse);
                })
                .AddMemoryStreams("Chat");
        })
        .AddSqlRepositories(options =>
        {
            options.ConnectionString = builder.Configuration.GetConnectionString("Application")!;
        });
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();