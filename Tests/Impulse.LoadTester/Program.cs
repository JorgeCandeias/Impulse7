using Impulse.Grains;
using Impulse.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBomber.CSharp;
using Orleans.Configuration;
using Spectre.Console;

var builder = Host.CreateApplicationBuilder();

// link up the global appsettings file
builder.Configuration.AddJsonFile("appsettings.global.json", false);

// link up the environment to a configuration key
builder.Environment.EnvironmentName = builder.Configuration["Environment"]!;

// suppress all client logging
builder.Logging.ClearProviders();

// add services for development
if (builder.Environment.IsDevelopment())
{
    // add orleans for development
    builder.UseOrleansClient(orleans =>
    {
        orleans.UseLocalhostClustering();
    });
}

// add services for production
else
{
    builder.UseOrleansClient(orleans =>
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
            });
    });
}

// add orleans services for all environments
builder.UseOrleansClient(orleans =>
{
    orleans
        .AddMemoryStreams("Chat");
});

using var host = builder.Build();

await host.StartAsync();

var client = host.Services.GetRequiredService<IClusterClient>();
var room = "overload";
var userNames = Enumerable.Range(1, 100).Select(x => Guid.NewGuid().ToString()).ToArray();
var messages = Enumerable.Range(1, 100).Select(x => Guid.NewGuid().ToString()).ToArray();
var rate = 100;

// warm up all the users in all the rooms
foreach (var userName in userNames)
{
    var user = await client.GetChatUsersIndexGrain().GetOrAdd(userName);

    await client.GetActiveChatRoomGrain(room).Join(user);
}

var messageScenario = Scenario
    .Create("Message", async context =>
    {
        try
        {
            var userName = userNames[Random.Shared.Next(userNames.Length)];
            var message = messages[Random.Shared.Next(messages.Length)];
            await client.GetActiveChatRoomGrain(room).Message(new ChatMessage(Guid.NewGuid(), room, userName, message));
        }
        catch
        {
            return Response.Fail();
        }

        return Response.Ok();
    })
    .WithLoadSimulations(Simulation.Inject(rate, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30)));

var historyScenario = Scenario
    .Create("History", async context =>
    {
        try
        {
            var userName = userNames[Random.Shared.Next(userNames.Length)];
            var message = messages[Random.Shared.Next(messages.Length)];
            await client.GetActiveChatRoomGrain(room).GetMessages();
        }
        catch
        {
            return Response.Fail();
        }

        return Response.Ok();
    })
    .WithLoadSimulations(Simulation.Inject(rate, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30)));

do
{
    Console.WriteLine("Type [ENTER] to start load testing or [ESC] to quit");
    var key = Console.ReadKey();
    if (key.Key == ConsoleKey.Escape)
    {
        break;
    }
    if (key.Key == ConsoleKey.Enter)
    {
        NBomberRunner.RegisterScenarios(messageScenario).Run();
        NBomberRunner.RegisterScenarios(historyScenario).Run();
        NBomberRunner.RegisterScenarios(messageScenario, historyScenario).Run();
    }
} while (true);

await host.StopAsync();