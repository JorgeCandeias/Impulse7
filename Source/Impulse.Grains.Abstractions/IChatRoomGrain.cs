using Impulse.Core;

namespace Impulse.Grains;

public interface IChatRoomGrain : IGrainWithStringKey
{
    Task Join(string user);

    Task Leave(string user);

    Task Message(ChatMessage message);

    [AlwaysInterleave]
    Task<ImmutableArray<ChatMessage>> GetHistory();

    [AlwaysInterleave]
    Task<ImmutableArray<ChatUser>> GetUsers();
}

public static class ChatRoomGrainFactoryExtensions
{
    public static IChatRoomGrain GetChatRoomGrain(this IGrainFactory factory, string room)
    {
        Guard.IsNotNull(factory);
        Guard.IsNotNullOrWhiteSpace(room);

        return factory.GetGrain<IChatRoomGrain>(room);
    }
}