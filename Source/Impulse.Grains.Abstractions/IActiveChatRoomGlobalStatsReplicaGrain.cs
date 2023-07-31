using Impulse.Core;

namespace Impulse.Grains;

public interface IActiveChatRoomGlobalStatsReplicaGrain : IGrainWithStringKey
{
    Task<ActiveChatRoomClusterStats> GetStats();
}

public static class ActiveChatRoomGlobalStatsReplicaGrainFactoryExtensions
{
    public static IActiveChatRoomGlobalStatsReplicaGrain GetActiveChatRoomGlobalStatsReplicaGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IActiveChatRoomGlobalStatsReplicaGrain>("DEFAULT");
    }
}