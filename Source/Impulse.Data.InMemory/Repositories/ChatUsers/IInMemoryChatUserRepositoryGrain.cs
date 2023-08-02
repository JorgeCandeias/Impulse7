namespace Impulse.Data.InMemory.Repositories.ChatUsers;

internal interface IInMemoryChatUserRepositoryGrain : IGrainWithStringKey
{
    public Task<IEnumerable<ChatUser>> GetAll();

    public Task<ChatUser> Save(ChatUser room);

    public Task<ChatUser?> TryGetByGuid(Guid guid);

    public Task<ChatUser?> TryGetByName(string name);

    public Task<Guid?> TryGetETagByGuid(Guid guid);

    public Task Remove(Guid guid, Guid etag);
}

internal static class InMemoryChatUserRepositoryGrainFactoryExtensions
{
    public static IInMemoryChatUserRepositoryGrain GetInMemoryChatUserRepositoryGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IInMemoryChatUserRepositoryGrain>("DEFAULT");
    }
}