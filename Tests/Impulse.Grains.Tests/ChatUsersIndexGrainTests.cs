namespace Impulse.Grains.Tests;

[Collection(nameof(TestClusterCollection))]
public class ChatUsersIndexGrainTests
{
    public ChatUsersIndexGrainTests(TestClusterFixture fixture)
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
        var added = await _fixture.TestCluster.Client.GetChatUsersIndexGrain().GetOrAdd(name);
        var single = await _fixture.TestCluster.Client.GetChatUsersIndexGrain().TryGetByGuid(added.Guid);
        var all = await _fixture.TestCluster.Client.GetChatUsersIndexGrain().GetAll();

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
        var added = await _fixture.TestCluster.Client.GetChatUsersIndexGrain().GetOrAdd(name);

        // act
        await _fixture.TestCluster.Client.GetChatUsersIndexGrain().Remove(added.Guid, added.ETag);
        var single = await _fixture.TestCluster.Client.GetChatUsersIndexGrain().TryGetByGuid(added.Guid);
        var all = await _fixture.TestCluster.Client.GetChatUsersIndexGrain().GetAll();

        // assert
        Assert.Null(single);
        Assert.DoesNotContain(added, all);
    }
}