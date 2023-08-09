using System.Threading.Channels;

namespace Impulse.Grains.Tests;

[Collection(nameof(TestClusterCollection))]
public class AsyncEnumerableBroadcastGrainTests
{
    public AsyncEnumerableBroadcastGrainTests(TestClusterFixture fixture)
    {
        _fixture = fixture;
    }

    private readonly TestClusterFixture _fixture;

    [Fact]
    public async Task EnumeratesPublishedValues()
    {
        // arrange
        var expected = new[] { 1, 2, 3, 4, 5 };
        var timeout = TimeSpan.FromSeconds(10);
        var key = Guid.NewGuid().ToString();
        var result = new List<int>();

        // act
        var enumeration = Task.Run(async () =>
        {
            await foreach (var value in _fixture.TestCluster.Client.GetGrain<IAsyncEnumerableBroadcastGrain>(key).Enumerate())
            {
                result.Add(value);
            }
        });

        // allow to async loop above to establish itself
        await Task.Delay(100);

        foreach (var value in expected)
        {
            await _fixture.TestCluster.Client.GetGrain<IAsyncEnumerableBroadcastGrain>(key).Publish(value);
        }

        await _fixture.TestCluster.Client.GetGrain<IAsyncEnumerableBroadcastGrain>(key).Complete();

        await enumeration.WaitAsync(timeout);

        // assert
        Assert.Equal(expected, result);
    }
}

public interface IAsyncEnumerableBroadcastGrain : IGrainWithStringKey
{
    ValueTask Publish(int value);

    ValueTask Complete();

    IAsyncEnumerable<int> Enumerate();
}

internal class AsyncEnumerableBroadcastGrain : Grain, IAsyncEnumerableBroadcastGrain
{
    private readonly HashSet<Channel<int>> _channels = new();

    public async ValueTask Publish(int value)
    {
        foreach (var channel in _channels)
        {
            await channel.Writer.WriteAsync(value);
        }
    }

    public ValueTask Complete()
    {
        foreach (var channel in _channels)
        {
            channel.Writer.Complete();
        }

        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<int> Enumerate()
    {
        var channel = Channel.CreateUnbounded<int>();

        _channels.Add(channel);

        await foreach (var value in channel.Reader.ReadAllAsync())
        {
            yield return value;
        }

        _channels.Remove(channel);
    }
}