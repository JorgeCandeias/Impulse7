using Impulse.Data.InMemory;
using Microsoft.Extensions.Configuration;

namespace Impulse.Grains.Tests;

public sealed class TestClusterFixture
{
    public TestClusterFixture()
    {
        TestCluster = new TestClusterBuilder()
            .AddSiloBuilderConfigurator<TestSiloConfigurator>()
            .AddClientBuilderConfigurator<TestClientConfigurator>()
            .Build();

        TestCluster.Deploy();
    }

    public TestCluster TestCluster { get; }

    private class TestSiloConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder
                .AddMemoryGrainStorageAsDefault()
                .AddMemoryGrainStorage("PubSubStore")
                .AddMemoryStreams("Chat")
                .UseInMemoryReminderService()
                .ConfigureServices(services =>
                {
                    services.AddInMemoryRepositories();
                });
        }
    }

    private class TestClientConfigurator : IClientBuilderConfigurator
    {
        public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
        {
            clientBuilder
                .AddMemoryStreams("Chat");
        }
    }
}