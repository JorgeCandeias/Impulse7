using Impulse.Core;

namespace Impulse.Grains;

public interface IChatRoomsIndexGrain : IGrainWithStringKey
{
    Task<ChatRoom> GetOrAdd(string name);

    Task<ChatRoom?> TryGetByGuid(Guid guid);

    Task<ImmutableArray<ChatRoom>> GetAll();

    Task Remove(Guid guid, Guid etag);
}

public static class ChatRoomsIndexGrainFactoryExtensions
{
    public static IChatRoomsIndexGrain GetChatRoomsIndexGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IChatRoomsIndexGrain>("DEFAULT");
    }
}