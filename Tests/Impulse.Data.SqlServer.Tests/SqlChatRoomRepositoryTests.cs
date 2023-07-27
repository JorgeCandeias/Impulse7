using System.Transactions;

namespace Impulse.Data.SqlServer.Tests;

[Collection(nameof(TestHostCollection))]
public class SqlChatRoomRepositoryTests
{
    public SqlChatRoomRepositoryTests(TestHostFixture fixture)
    {
        _fixture = fixture;
    }

    private readonly TestHostFixture _fixture;

    [Fact]
    [Trait("Category", "Database")]
    public async Task GetsOrAddsChatRoom()
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

        // assert - result 1
        Assert.NotNull(result1);
        Assert.Equal(name1, result1.Name);
        Assert.True(result1.Created >= start);
        Assert.True(result1.Created <= end);

        // assert - result 2
        Assert.NotNull(result2);
        Assert.Equal(result2.Name, result1.Name);
        Assert.Equal(result1.Created, result2.Created);

        // assert - result 3
        Assert.NotNull(result3);
        Assert.Equal(name2, result3.Name);
        Assert.True(result3.Created >= start);
        Assert.True(result3.Created <= end);
        Assert.True(result3.Created > result2.Created);

        // assert - result 4
        Assert.NotNull(result4);
        Assert.Equal(result3.Name, result4.Name);
        Assert.Equal(result3.Created, result3.Created);

        // assert - result 5
        Assert.NotNull(result5);
        Assert.Equal(name3, result5.Name);
        Assert.True(result5.Created >= start);
        Assert.True(result5.Created <= end);
        Assert.True(result5.Created > result4.Created);
    }

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
}