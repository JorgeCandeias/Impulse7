namespace Impulse.Data.SqlServer.Tests;

public sealed class TestHostFixture : IDisposable
{
    public TestHostFixture()
    {
        // arrange
        TestHost = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSqlRepositories(options =>
                {
                    options.ConnectionString = Settings.ConnectionString;
                });
            })
            .Build();
    }

    public IHost TestHost { get; }

    public void Dispose()
    {
        TestHost.Dispose();
    }
}