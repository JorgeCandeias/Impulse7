using Impulse.Core;

namespace Impulse.Grains;

public interface IChatRoomGlobalStatsGrain : IGrainWithStringKey
{
    Task Publish(ChatRoomSiloStats stats);

    Task<(ChatRoomClusterStats Stats, Guid Version)> GetStats();

    Task<(ChatRoomClusterStats Stats, Guid Version)?> WaitForStats(Guid current);
}

public static class ChatRoomGlobalStatsGrainFactoryExtensions
{
    public static IChatRoomGlobalStatsGrain GetChatRoomGlobalStatsGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IChatRoomGlobalStatsGrain>("DEFAULT");
    }
}