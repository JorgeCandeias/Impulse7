namespace Impulse.Grains;

public interface IActiveChatRoomTracingGrain : IGrainWithStringKey
{
    Task Start();

    Task Stop();
}

public static class ActiveChatRoomTracingGrainFactoryExtensions
{
    public static IActiveChatRoomTracingGrain GetActiveChatRoomTracingGrain(this IGrainFactory factory, string room)
    {
        Guard.IsNotNull(factory);
        Guard.IsNotNullOrWhiteSpace(room);

        return factory.GetGrain<IActiveChatRoomTracingGrain>(room);
    }
}