using Impulse.Models;
using Orleans.Streams;

namespace Impulse.Grains.Tests;

[Collection(nameof(TestClusterCollection))]
public class ActiveChatRoomGrainTests
{
    public ActiveChatRoomGrainTests(TestClusterFixture fixture)
    {
        _fixture = fixture;
    }

    private readonly TestClusterFixture _fixture;

    [Fact]
    public async Task JoinsUser()
    {
        // arrange
        var userName = Guid.NewGuid().ToString();
        var roomName = Guid.NewGuid().ToString();
        var user = await _fixture.TestCluster.Client.GetChatUsersIndexGrain().GetOrAdd(userName);

        // act
        var completion = new TaskCompletionSource<ChatMessage>();
        await _fixture.TestCluster.Client
            .GetStreamProvider("Chat")
            .GetStream<ChatMessage>(roomName)
            .SubscribeAsync((message, token) =>
            {
                completion.SetResult(message);

                return Task.CompletedTask;
            });

        await _fixture.TestCluster.Client.GetActiveChatRoomGrain(roomName).Join(user);

        var result = await completion.Task.WaitAsync(TimeSpan.FromSeconds(10));

        // assert
        Assert.NotNull(result);
        Assert.Equal("System", result.User);
        Assert.Equal(roomName, result.Room);
        Assert.Equal($"{userName} joined chat room {roomName}", result.Text);
    }

    [Fact]
    public async Task LeavesUser()
    {
        // arrange
        var userName = Guid.NewGuid().ToString();
        var roomName = Guid.NewGuid().ToString();
        var user = await _fixture.TestCluster.Client.GetChatUsersIndexGrain().GetOrAdd(userName);
        await _fixture.TestCluster.Client.GetActiveChatRoomGrain(roomName).Join(user);

        // act
        var completion = new TaskCompletionSource<ChatMessage>();
        await _fixture.TestCluster.Client
            .GetStreamProvider("Chat")
            .GetStream<ChatMessage>(roomName)
            .SubscribeAsync((message, token) =>
            {
                // skip the joined message in case of race condition
                if (message.Text.Contains("joined"))
                {
                    return Task.CompletedTask;
                }

                completion.SetResult(message);

                return Task.CompletedTask;
            });

        await _fixture.TestCluster.Client.GetActiveChatRoomGrain(roomName).Leave(user);

        var result = await completion.Task.WaitAsync(TimeSpan.FromSeconds(10));

        // assert
        Assert.NotNull(result);
        Assert.Equal("System", result.User);
        Assert.Equal(roomName, result.Room);
        Assert.Equal($"{userName} left chat room {roomName}", result.Text);
    }

    [Fact]
    public async Task Messages()
    {
        // arrange
        var userName = Guid.NewGuid().ToString();
        var roomName = Guid.NewGuid().ToString();
        var user = await _fixture.TestCluster.Client.GetChatUsersIndexGrain().GetOrAdd(userName);

        // act
        var completion = new TaskCompletionSource<ChatMessage>();
        await _fixture.TestCluster.Client
            .GetStreamProvider("Chat")
            .GetStream<ChatMessage>(roomName)
            .SubscribeAsync((message, token) =>
            {
                completion.SetResult(message);

                return Task.CompletedTask;
            });

        var message = new ChatMessage(Guid.NewGuid(), roomName, userName, "Some Text");
        await _fixture.TestCluster.Client.GetActiveChatRoomGrain(roomName).Message(message);

        var result = await completion.Task.WaitAsync(TimeSpan.FromSeconds(10));

        // assert
        Assert.NotNull(result);
        Assert.Equal(message.Guid, result.Guid);
        Assert.Equal(message.User, result.User);
        Assert.Equal(message.Room, result.Room);
        Assert.Equal(message.Text, result.Text);
    }
}