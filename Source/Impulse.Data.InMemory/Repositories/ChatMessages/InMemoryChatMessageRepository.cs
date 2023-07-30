using Impulse.Core;

namespace Impulse.Data.InMemory.Repositories.ChatMessages;

internal class InMemoryChatMessageRepository : IChatMessageRepository
{
    public InMemoryChatMessageRepository(IGrainFactory factory)
    {
        _factory = factory;
    }

    private readonly IGrainFactory _factory;

    public Task<ChatMessage> Save(ChatMessage message, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatMessageRepositoryGrain().Save(message);
    }

    public Task<Guid?> TryGetETagByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatMessageRepositoryGrain().TryGetETagByGuid(guid);
    }

    public Task<ChatMessage?> TryGetByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatMessageRepositoryGrain().TryGetByGuid(guid);
    }

    public Task<IEnumerable<ChatMessage>> GetAll(CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatMessageRepositoryGrain().GetAll();
    }

    public Task Remove(Guid guid, Guid etag, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatMessageRepositoryGrain().Remove(guid, etag);
    }

    public Task<IEnumerable<ChatMessage>> GetLatestCreatedByRoom(string room, int count, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatMessageRepositoryGrain().GetLatestCreatedByRoom(room, count);
    }
}