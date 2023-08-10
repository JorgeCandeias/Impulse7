namespace Impulse.Grains.Tests;

[Collection(nameof(TestClusterCollection))]
internal class MigratingCounterGrainTests
{
    public MigratingCounterGrainTests(TestClusterFixture fixture)
    {
        _fixture = fixture;
    }

    private readonly TestClusterFixture _fixture;

    [Fact]
    public async Task Migrates()
    {
    }
}

public interface IMigratingCounterGrain : IGrainWithStringKey
{
    ValueTask<int> Add(int value);
}

internal class MigratingCounterGrain : Grain, IMigratingCounterGrain
{
    private int _count;

    public ValueTask<int> Add(int value)
    {
        _count += value;

        return ValueTask.FromResult(_count);
    }
}