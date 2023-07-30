using Impulse.Data.InMemory;
using Impulse.Data.SqlServer;
using Impulse.WebApi.Models;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

// link up the global appsettings file
builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.global.json"), false);

// link up the environment to a configuration key
builder.Environment.EnvironmentName = builder.Configuration["Environment"]!;

// add common services
builder.Services.AddAutoMapper(options =>
{
    options.AddProfile<ApiModelsProfile>();
});

// add repositories as appropriate for the environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddInMemoryRepositories();
}
else
{
    builder.Services.AddSqlRepositories(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("Application")!;
    });
}

// add web api services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add orleans services
builder.Services.AddOrleansClient(orleans =>
{
    if (builder.Environment.IsDevelopment())
    {
        orleans.UseLocalhostClustering();
    }
    else
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
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();