using Impulse.Core;

namespace Impulse.Data;

public interface IChatRoomsIndexGrain : IGrainWithStringKey
{
    Task<ChatRoom> GetOrAdd(string name);

    Task<ChatRoom?> TryGetByGuid(Guid guid);
}

public static class ChatRoomsIndexGrainFactoryExtensions
{
    public static IChatRoomsIndexGrain GetChatRoomsIndexGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IChatRoomsIndexGrain>("DEFAULT");
    }
}