using Impulse.Core;

namespace Impulse.Data.InMemory.Repositories.ChatRooms;

internal class InMemoryChatRoomRepository : IChatRoomRepository
{
    public InMemoryChatRoomRepository(IGrainFactory factory)
    {
        _factory = factory;
    }

    private readonly IGrainFactory _factory;

    public Task<IEnumerable<ChatRoom>> GetAll(CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatRoomRepositoryGrain().GetAll();
    }

    public Task<ChatRoom> Save(ChatRoom room, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatRoomRepositoryGrain().Save(room);
    }

    public Task<ChatRoom?> TryGetByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatRoomRepositoryGrain().TryGetByGuid(guid);
    }

    public Task<ChatRoom?> TryGetByName(string name, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatRoomRepositoryGrain().TryGetByName(name);
    }

    public Task<Guid?> TryGetETagByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatRoomRepositoryGrain().TryGetETagByGuid(guid);
    }

    public Task Remove(Guid guid, Guid etag, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatRoomRepositoryGrain().Remove(guid, etag);
    }
}