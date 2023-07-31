using Impulse.Core;

namespace Impulse.Grains;

public interface IActiveChatRoomGlobalStatsGrain : IGrainWithStringKey
{
    Task Publish(ActiveChatRoomSiloStats stats);

    Task<(ActiveChatRoomClusterStats Stats, Guid Version)> GetStats();

    Task<(ActiveChatRoomClusterStats Stats, Guid Version)?> WaitForStats(Guid current);
}

public static class ActiveChatRoomGlobalStatsGrainFactoryExtensions
{
    public static IActiveChatRoomGlobalStatsGrain GetActiveChatRoomGlobalStatsGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IActiveChatRoomGlobalStatsGrain>("DEFAULT");
    }
}