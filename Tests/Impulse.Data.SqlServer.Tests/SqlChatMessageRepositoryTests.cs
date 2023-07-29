using Impulse.Core;
using System.Transactions;

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
    public async Task AddsMessage()
    {
        // arrange
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var id = Guid.NewGuid().ToString();
        var name1 = Guid.NewGuid().ToString();
        var name2 = Guid.NewGuid().ToString();
        var name3 = Guid.NewGuid().ToString();
        var room = Guid.NewGuid().ToString();
        var user = Guid.NewGuid().ToString();
        var text = Guid.NewGuid().ToString();
        var created = DateTimeOffset.UtcNow;
        var message = new ChatMessage(Guid.NewGuid(), room, user, text, created);

        // act
        var service = _fixture.TestHost.Services.GetRequiredService<IChatMessageRepository>();
        var result = await service.AddMessage(message);

        // assert
        Assert.NotNull(result);
    }

    /*
    [Fact]
    [Trait("Category", "Database")]
    public async Task GetsAll()
    {
        // arrange
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var name1 = Guid.NewGuid().ToString();
        var name2 = Guid.NewGuid().ToString();
        var name3 = Guid.NewGuid().ToString();

        // act
        var service = _fixture.TestHost.Services.GetRequiredService<IChatRoomRepository>();
        var start = DateTimeOffset.UtcNow.AddSeconds(-1);
        var result1 = await service.GetOrAdd(name1);
        var result2 = await service.GetOrAdd(name1);
        var result3 = await service.GetOrAdd(name2);
        var result4 = await service.GetOrAdd(name2);
        var result5 = await service.GetOrAdd(name3);
        var end = DateTimeOffset.UtcNow.AddSeconds(1);
        var all = await service.GetAll();

        // assert
        Assert.NotNull(all);

        var list = all.ToList();
        Assert.Equal(3, list.Count);
        Assert.Contains(list, x => x.Name == name1 && x.Created >= start && x.Created <= end);
        Assert.Contains(list, x => x.Name == name2 && x.Created >= start && x.Created <= end);
        Assert.Contains(list, x => x.Name == name3 && x.Created >= start && x.Created <= end);
    }
    */
}