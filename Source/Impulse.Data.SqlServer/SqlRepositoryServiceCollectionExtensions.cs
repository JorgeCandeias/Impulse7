using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Impulse.Data.SqlServer;

public static class SqlRepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddSqlRepository(this IServiceCollection services, Action<SqlRepositoryOptions> configure)
    {
        Guard.IsNotNull(services);

        return services
            .AddSingleton<IChatRepository, SqlChatRepository>()
            .AddOptions<SqlRepositoryOptions>()
            .Configure(configure)
            .ValidateDataAnnotations()
            .Services;
    }
}
