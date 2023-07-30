using Impulse.WebApi.Models;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

// fix the environment
builder.Environment.EnvironmentName = Environments.Development;

// add common services
builder.Services.AddAutoMapper(options =>
{
    options.AddProfile<ApiModelsProfile>();
});

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