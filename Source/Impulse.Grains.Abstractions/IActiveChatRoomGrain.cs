﻿using Impulse.Models;

namespace Impulse.Grains;

public interface IActiveChatRoomGrain : IGrainWithStringKey
{
    Task Join(ChatUser user);

    Task Leave(ChatUser user);

    Task Message(ChatMessage message);

    Task<ImmutableArray<ChatMessage>> GetMessages();

    Task<ImmutableArray<ChatUser>> GetUsers();
}

public static class ActiveChatRoomGrainFactoryExtensions
{
    public static IActiveChatRoomGrain GetActiveChatRoomGrain(this IGrainFactory factory, string room)
    {
        Guard.IsNotNull(factory);
        Guard.IsNotNullOrWhiteSpace(room);

        return factory.GetGrain<IActiveChatRoomGrain>(room);
    }
}