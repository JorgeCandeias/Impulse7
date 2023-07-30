﻿using Impulse.Data.InMemory.Repositories.ChatMessages;
using Impulse.Data.InMemory.Repositories.ChatRooms;
using Impulse.Data.InMemory.Repositories.ChatUsers;
using Microsoft.Extensions.DependencyInjection;

namespace Impulse.Data.InMemory;

public static class InMemoryRepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
    {
        Guard.IsNotNull(services);

        return services
            .AddSingleton<IChatRoomRepository, InMemoryChatRoomRepository>()
            .AddSingleton<IChatUserRepository, InMemoryChatUserRepository>()
            .AddSingleton<IChatMessageRepository, InMemoryChatMessageRepository>();
    }
}