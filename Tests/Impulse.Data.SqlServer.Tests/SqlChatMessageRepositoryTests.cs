using Impulse.Core;

namespace Impulse.Data.SqlServer.Tests;

[Collection(nameof(TestHostCollection))]
public class SqlChatMessageRepositoryTests
{
    public SqlChatMessageRepositoryTests(TestHostFixture fixture)
    {
        _fixture = fixture;
    }

    private readonly TestHostFixture _fixture;

    [Fact]
    [Trait("Category", "Database")]
    public async Task SavesNewChatMessage()
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // arrange
        var room = Guid.NewGuid().ToString();
        await _fixture.TestHost.Services
            .GetRequiredService<IChatRoomRepository>()
            .Save(new ChatRoom(Guid.NewGuid(), room));

        var user = Guid.NewGuid().ToString();
        await _fixture.TestHost.Services
            .GetRequiredService<IChatUserRepository>()
            .Save(new ChatUser(Guid.NewGuid(), user));

        var service = _fixture.TestHost.Services.GetRequiredService<IChatMessageRepository>();
        var guid = Guid.NewGuid();
        var text = Guid.NewGuid().ToString();
        var candidate = new ChatMessage(guid, room, user, text);

        // act
        var saved = await service.Save(candidate);
        var read = await service.TryGetByGuid(guid);
        var all = await service.GetAll();
        var latest = await service.GetLatestCreatedByRoom(room, 10);

        // assert - saved
        Assert.NotNull(saved);
        Assert.Equal(candidate.Guid, saved.Guid);
        Assert.Equal(candidate.Room, saved.Room);
        Assert.Equal(candidate.User, saved.User);
        Assert.NotEqual(candidate.ETag, saved.ETag);

        // assert - read by guid
        Assert.Equal(saved, read);

        // assert - all
        Assert.NotNull(all);
        Assert.Contains(saved, all);

        // assert - latest
        Assert.NotNull(latest);
        Assert.Contains(saved, latest);
    }

    [Fact]
    [Trait("Category", "Database")]
    public async Task UpdatesChatMessage()
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // arrange
        var room = Guid.NewGuid().ToString();
        await _fixture.TestHost.Services
            .GetRequiredService<IChatRoomRepository>()
            .Save(new ChatRoom(Guid.NewGuid(), room));

        var user = Guid.NewGuid().ToString();
        await _fixture.TestHost.Services
            .GetRequiredService<IChatUserRepository>()
            .Save(new ChatUser(Guid.NewGuid(), user));

        var service = _fixture.TestHost.Services.GetRequiredService<IChatMessageRepository>();
        var guid = Guid.NewGuid();
        var text = Guid.NewGuid().ToString();
        var revision = Guid.NewGuid().ToString();
        var existing = await service.Save(new ChatMessage(guid, room, user, text));

        // act
        var saved = await service.Save(existing with { Text = revision });
        var read = await service.TryGetByGuid(guid);
        var all = await service.GetAll();
        var latest = await service.GetLatestCreatedByRoom(room, 10);

        // assert - saved
        Assert.NotNull(saved);
        Assert.Equal(guid, saved.Guid);
        Assert.Equal(user, saved.User);
        Assert.Equal(revision, saved.Text);
        Assert.Equal(existing.Created, saved.Created);
        Assert.NotEqual(existing.ETag, saved.ETag);

        // assert - read by guid
        Assert.Equal(saved, read);

        // assert - all
        Assert.DoesNotContain(existing, all);
        Assert.Contains(saved, all);

        // assert - latest
        Assert.DoesNotContain(existing, latest);
        Assert.Contains(saved, latest);
    }

    [Fact]
    [Trait("Category", "Database")]
    public async Task RemovesChatMessage()
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // arrange
        var guid = Guid.NewGuid();
        var room = Guid.NewGuid().ToString();
        var user = Guid.NewGuid().ToString();
        var text = Guid.NewGuid().ToString();
        await _fixture.TestHost.Services.GetRequiredService<IChatRoomRepository>().Save(new ChatRoom(Guid.NewGuid(), room));
        await _fixture.TestHost.Services.GetRequiredService<IChatUserRepository>().Save(new ChatUser(Guid.NewGuid(), user));
        var service = _fixture.TestHost.Services.GetRequiredService<IChatMessageRepository>();
        var candidate = new ChatMessage(guid, room, user, text, DateTimeOffset.MinValue, DateTimeOffset.MinValue, Guid.Empty);
        var saved = await service.Save(candidate);

        // act
        await service.Remove(saved.Guid, saved.ETag);
        var read1 = await service.TryGetByGuid(guid);
        var all = await service.GetAll();

        // assert
        Assert.Null(read1);
        Assert.DoesNotContain(saved, all);
    }
}