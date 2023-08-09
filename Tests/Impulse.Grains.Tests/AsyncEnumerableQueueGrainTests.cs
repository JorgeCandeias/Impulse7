using System.Threading.Channels;

namespace Impulse.Grains.Tests;

[Collection(nameof(TestClusterCollection))]
public class AsyncEnumerableQueueGrainTests
{
    public AsyncEnumerableQueueGrainTests(TestClusterFixture fixture)
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
            await foreach (var value in _fixture.TestCluster.Client.GetGrain<IAsyncEnumerableQueueGrain>(key).Enumerate())
            {
                result.Add(value);
            }
        });

        foreach (var value in expected)
        {
            await _fixture.TestCluster.Client.GetGrain<IAsyncEnumerableQueueGrain>(key).Publish(value);
        }

        await _fixture.TestCluster.Client.GetGrain<IAsyncEnumerableQueueGrain>(key).Complete();

        await enumeration.WaitAsync(timeout);

        // assert
        Assert.Equal(expected, result);
    }
}

public interface IAsyncEnumerableQueueGrain : IGrainWithStringKey
{
    ValueTask Publish(int value);

    ValueTask Complete();

    IAsyncEnumerable<int> Enumerate();
}

internal class AsyncEnumerableQueueGrain : Grain, IAsyncEnumerableQueueGrain
{
    private readonly Channel<int> _channel = Channel.CreateUnbounded<int>();

    public ValueTask Publish(int value)
    {
        return _channel.Writer.WriteAsync(value);
    }

    public ValueTask Complete()
    {
        _channel.Writer.Complete();

        return ValueTask.CompletedTask;
    }

    public IAsyncEnumerable<int> Enumerate()
    {
        return _channel.Reader.ReadAllAsync();
    }
}