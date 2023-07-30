using Impulse.Core;

namespace Impulse.Data;

public interface IChatUsersIndexGrain : IGrainWithStringKey
{
    Task<ChatUser> GetOrAdd(string name);

    Task<ChatUser?> TryGetByGuid(Guid guid);
}

public static class ChatUsersIndexGrainFactoryExtensions
{
    public static IChatUsersIndexGrain GetChatUsersIndexGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IChatUsersIndexGrain>("DEFAULT");
    }
}