using Impulse.Core;

namespace Impulse.Grains;

public interface IActiveChatRoomLocalStatsGrain : IGrainWithStringKey
{
    Task Publish(ActiveChatRoomStats stats);
}

public static class ActiveChatRoomLocalStatsGrainFactoryExtensions
{
    public static IActiveChatRoomLocalStatsGrain GetActiveChatRoomLocalStatsGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IActiveChatRoomLocalStatsGrain>("DEFAULT");
    }
}