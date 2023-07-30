namespace Impulse.Grains.Tests;

[Collection(nameof(TestClusterCollection))]
public class ChatRoomsIndexGrainTests
{
    public ChatRoomsIndexGrainTests(TestClusterFixture fixture)
    {
        _fixture = fixture;
    }

    private readonly TestClusterFixture _fixture;

    [Fact]
    public async Task AddsAndGetsUser()
    {
        // arrange
        var name = Guid.NewGuid().ToString();

        // act
        var added = await _fixture.TestCluster.Client.GetChatRoomsIndexGrain().GetOrAdd(name);
        var single = await _fixture.TestCluster.Client.GetChatRoomsIndexGrain().TryGetByGuid(added.Guid);
        var all = await _fixture.TestCluster.Client.GetChatRoomsIndexGrain().GetAll();

        // assert
        Assert.NotNull(added);
        Assert.NotEqual(Guid.Empty, added.Guid);
        Assert.Equal(name, added.Name);
        Assert.Equal(added, single);
        Assert.Contains(added, all);
    }

    [Fact]
    public async Task RemovesUser()
    {
        // arrange
        var name = Guid.NewGuid().ToString();
        var added = await _fixture.TestCluster.Client.GetChatRoomsIndexGrain().GetOrAdd(name);

        // act
        await _fixture.TestCluster.Client.GetChatRoomsIndexGrain().Remove(added.Guid, added.ETag);
        var single = await _fixture.TestCluster.Client.GetChatRoomsIndexGrain().TryGetByGuid(added.Guid);
        var all = await _fixture.TestCluster.Client.GetChatRoomsIndexGrain().GetAll();

        // assert
        Assert.Null(single);
        Assert.DoesNotContain(added, all);
    }
}