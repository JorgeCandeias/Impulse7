namespace Impulse.Data.InMemory.Repositories.ChatRooms;

internal interface IInMemoryChatRoomRepositoryGrain : IGrainWithStringKey
{
    public Task<IEnumerable<ChatRoom>> GetAll();

    public Task<ChatRoom> Save(ChatRoom room);

    public Task<ChatRoom?> TryGetByGuid(Guid guid);

    public Task<ChatRoom?> TryGetByName(string name);

    public Task<Guid?> TryGetETagByGuid(Guid guid);

    public Task Remove(Guid guid, Guid etag);
}

internal static class InMemoryChatRoomRepositoryGrainFactoryExtensions
{
    public static IInMemoryChatRoomRepositoryGrain GetInMemoryChatRoomRepositoryGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IInMemoryChatRoomRepositoryGrain>("DEFAULT");
    }
}