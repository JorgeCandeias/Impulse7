using Impulse.Core;

namespace Impulse.Grains;

public interface IChatRoomGlobalStatsReplicaGrain : IGrainWithStringKey
{
    Task<ChatRoomClusterStats> GetStats();
}

public static class ChatRoomGlobalStatsReplicaGrainFactoryExtensions
{
    public static IChatRoomGlobalStatsReplicaGrain GetChatRoomGlobalStatsReplicaGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IChatRoomGlobalStatsReplicaGrain>("DEFAULT");
    }
}