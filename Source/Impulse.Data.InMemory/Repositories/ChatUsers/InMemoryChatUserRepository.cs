namespace Impulse.Data.InMemory.Repositories.ChatUsers;

internal class InMemoryChatUserRepository : IChatUserRepository
{
    public InMemoryChatUserRepository(IGrainFactory factory)
    {
        _factory = factory;
    }

    private readonly IGrainFactory _factory;

    public Task<IEnumerable<ChatUser>> GetAll(CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatUserRepositoryGrain().GetAll();
    }

    public Task<ChatUser> Save(ChatUser user, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatUserRepositoryGrain().Save(user);
    }

    public Task<ChatUser?> TryGetByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatUserRepositoryGrain().TryGetByGuid(guid);
    }

    public Task<ChatUser?> TryGetByName(string name, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatUserRepositoryGrain().TryGetByName(name);
    }

    public Task<Guid?> TryGetETagByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatUserRepositoryGrain().TryGetETagByGuid(guid);
    }

    public Task Remove(Guid guid, Guid etag, CancellationToken cancellationToken = default)
    {
        return _factory.GetInMemoryChatUserRepositoryGrain().Remove(guid, etag);
    }
}