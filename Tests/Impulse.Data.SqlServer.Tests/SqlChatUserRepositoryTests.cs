using Impulse.Core;

namespace Impulse.Data.SqlServer.Tests;

[Collection(nameof(TestHostCollection))]
public class SqlChatUserRepositoryTests
{
    public SqlChatUserRepositoryTests(TestHostFixture fixture)
    {
        _fixture = fixture;
    }

    private readonly TestHostFixture _fixture;

    [Fact]
    [Trait("Category", "Database")]
    public async Task SavesNewChatUser()
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // arrange
        var service = _fixture.TestHost.Services.GetRequiredService<IChatUserRepository>();
        var guid = Guid.NewGuid();
        var name = Guid.NewGuid().ToString();
        var room = new ChatUser(guid, name, DateTimeOffset.MinValue, DateTimeOffset.MinValue, Guid.Empty);

        // act
        var saved = await service.Save(room);
        var read1 = await service.TryGetByGuid(guid);
        var read2 = await service.TryGetByName(name);
        var all = await service.GetAll();

        // assert - saved
        Assert.NotNull(saved);
        Assert.Equal(room.Guid, saved.Guid);
        Assert.Equal(room.Name, saved.Name);
        Assert.NotEqual(room.Created, saved.Created);
        Assert.NotEqual(room.Updated, saved.Updated);
        Assert.NotEqual(room.ETag, saved.ETag);

        // assert - read by guid
        Assert.Equal(saved, read1);

        // assert - read by name
        Assert.Equal(saved, read2);

        // assert - all
        Assert.NotNull(all);
        Assert.Contains(read1, all);
        Assert.Contains(read2, all);
    }

    [Fact]
    [Trait("Category", "Database")]
    public async Task UpdatesChatUser()
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // arrange
        var service = _fixture.TestHost.Services.GetRequiredService<IChatUserRepository>();
        var guid = Guid.NewGuid();
        var name = Guid.NewGuid().ToString();
        var rename = Guid.NewGuid().ToString();
        var existing = await service.Save(new ChatUser(guid, name, DateTimeOffset.MinValue, DateTimeOffset.MinValue, Guid.Empty));

        // act
        var saved = await service.Save(existing with { Name = rename });
        var read1 = await service.TryGetByGuid(guid);
        var read2 = await service.TryGetByName(name);
        var read3 = await service.TryGetByName(rename);
        var all = await service.GetAll();

        // assert - saved
        Assert.NotNull(saved);
        Assert.Equal(guid, saved.Guid);
        Assert.Equal(rename, saved.Name);
        Assert.Equal(existing.Created, saved.Created);
        Assert.NotEqual(existing.Updated, saved.Updated);
        Assert.NotEqual(existing.ETag, saved.ETag);

        // assert - read by guid
        Assert.Equal(saved, read1);

        // assert - read by name
        Assert.Null(read2);

        // assert - read by renamed
        Assert.Equal(saved, read3);

        // assert - all
        Assert.DoesNotContain(existing, all);
        Assert.Contains(saved, all);
    }
}