using CommunityToolkit.Diagnostics;
using System.Threading.Channels;

namespace Impulse.Grains.Tests;

[Collection(nameof(TestClusterCollection))]
public class AsyncEnumerableRewindableGrainTests
{
    public AsyncEnumerableRewindableGrainTests(TestClusterFixture fixture)
    {
        _fixture = fixture;
    }

    private readonly TestClusterFixture _fixture;

    [Fact]
    public async Task EnumeratesPublishedValues()
    {
        // arrange
        var timeout = TimeSpan.FromSeconds(10);
        var key = Guid.NewGuid().ToString();
        var result = new List<int>();
        var grain = _fixture.TestCluster.Client.GetGrain<IAsyncEnumerableRewindableGrain>(key);

        // act - publish history
        await grain.Publish(1);
        await grain.Publish(2);
        await grain.Publish(3);
        await grain.Publish(4);

        // act - subscribe from the past
        var from = 2;
        var enumeration = Task.Run(async () =>
        {
            await foreach (var value in _fixture.TestCluster.Client.GetGrain<IAsyncEnumerableRewindableGrain>(key).Enumerate(from))
            {
                result.Add(value);
            }
        });

        // act - allow to async loop above to establish itself
        await Task.Delay(100);

        // act - publish new values and complete
        await grain.Publish(5);
        await grain.Complete();

        // act - wait for the async loop to complet
        await enumeration.WaitAsync(timeout);

        // assert
        Assert.Equal(new[] { 3, 4, 5 }, result);
    }
}

public interface IAsyncEnumerableRewindableGrain : IGrainWithStringKey
{
    ValueTask Publish(int value);

    ValueTask Complete();

    IAsyncEnumerable<int> Enumerate(int from);
}

internal class AsyncEnumerableRewindableGrain : Grain, IAsyncEnumerableRewindableGrain
{
    private readonly List<int> _history = new();
    private readonly HashSet<Channel<int>> _channels = new();

    public async ValueTask Publish(int value)
    {
        _history.Add(value);

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

    public async IAsyncEnumerable<int> Enumerate(int from)
    {
        Guard.IsGreaterThanOrEqualTo(from, 0);

        // write the history synchronously to ensure we dont allow reentrancy to cause race conditions
        var channel = Channel.CreateUnbounded<int>();
        foreach (var value in _history.Skip(from))
        {
            if (!channel.Writer.TryWrite(value))
            {
                throw new InvalidOperationException();
            }
        }
        _channels.Add(channel);

        // now it is safe to enumerate as normal
        await foreach (var value in channel.Reader.ReadAllAsync())
        {
            yield return value;
        }

        _channels.Remove(channel);
    }
}