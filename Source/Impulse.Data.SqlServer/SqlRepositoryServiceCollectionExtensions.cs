using Impulse.Data.SqlServer.Models;
using Impulse.Data.SqlServer.Repositories;

namespace Impulse.Data.SqlServer;

public static class SqlRepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddSqlRepositories(this IServiceCollection services, Action<SqlRepositoryOptions> configure)
    {
        Guard.IsNotNull(services);
        Guard.IsNotNull(configure);

        return services
            .AddSingleton<IChatRoomRepository, SqlChatRoomRepository>()
            .AddSingleton<IChatUserRepository, SqlChatUserRepository>()
            .AddSingleton<IChatMessageRepository, SqlChatMessageRepository>()
            .AddAutoMapper(config =>
            {
                config.AddProfile<SqlRepositoryProfile>();
            })
            .AddOptions<SqlRepositoryOptions>()
            .Configure(configure)
            .ValidateDataAnnotations()
            .Services;
    }
}