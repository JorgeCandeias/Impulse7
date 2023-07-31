using Impulse.Data.InMemory;
using Impulse.Data.SqlServer;
using Impulse.WebApi.Models;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

// link up the global appsettings file
builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.global.json"), false);

// link up the environment to a configuration key
builder.Environment.EnvironmentName = builder.Configuration["Environment"]!;

// add services for any environment
builder.Services.AddAutoMapper(options =>
{
    options.AddProfile<ApiModelsProfile>();
});

// add services for development
if (builder.Environment.IsDevelopment())
{
    // add in-memory repositories for development
    builder.Services.AddInMemoryRepositories();

    // add orleans with development services
    builder.Services.AddOrleansClient(orleans =>
    {
        orleans.UseLocalhostClustering();
    });
}

// add services for production
else
{
    // add sql repositories for production
    builder.Services.AddSqlRepositories(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("Application")!;
    });

    // add orleans with production services
    builder.Services.AddOrleansClient(orleans =>
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
    });
}

// add web api services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();