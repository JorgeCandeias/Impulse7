using Impulse.Grains;
using Microsoft.Extensions.Hosting;

var host = Host
    .CreateDefaultBuilder()
    .UseOrleans(orleans =>
    {
        orleans
            .UseLocalhostClustering()
            .AddActivityGrainCallFilter(options =>
            {
                options.AllowedAssemblies.Add(typeof(ActivityGrainCallFilterOptions).Assembly);
            });
    })
    .Build();

await host.RunAsync();