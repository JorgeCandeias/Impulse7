using Impulse.Core;

namespace Impulse.Grains;

public interface IChatUsersIndexGrain : IGrainWithStringKey
{
    Task<ChatUser> GetOrAdd(string name);

    Task<ChatUser?> TryGetByGuid(Guid guid);

    Task<ImmutableArray<ChatUser>> GetAll();

    Task Remove(Guid guid, Guid etag);
}

public static class ChatUsersIndexGrainFactoryExtensions
{
    public static IChatUsersIndexGrain GetChatUsersIndexGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IChatUsersIndexGrain>("DEFAULT");
    }
}