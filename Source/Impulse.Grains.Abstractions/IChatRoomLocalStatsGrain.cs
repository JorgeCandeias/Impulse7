using Impulse.Core;

namespace Impulse.Grains;

public interface IChatRoomLocalStatsGrain : IGrainWithStringKey
{
    Task Publish(ChatRoomStats stats);
}

public static class ChatRoomLocalStatsGrainFactoryExtensions
{
    public static IChatRoomLocalStatsGrain GetChatRoomLocalStatsGrain(this IGrainFactory factory)
    {
        Guard.IsNotNull(factory);

        return factory.GetGrain<IChatRoomLocalStatsGrain>("DEFAULT");
    }
}