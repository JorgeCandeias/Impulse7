using CommunityToolkit.Diagnostics;
using Impulse.Core;
using Impulse.Data.SqlServer.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Impulse.Data.SqlServer.Repositories;

internal class SqlChatMessageRepository : IChatMessageRepository
{
    private readonly SqlRepositoryOptions _options;

    public SqlChatMessageRepository(IOptions<SqlRepositoryOptions> options)
    {
        _options = options.Value;
    }

    private SqlConnection CreateConnection() => new(_options.ConnectionString);

    public async Task<ChatMessage> AddMessage(ChatMessage message, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(message);

        using var connection = CreateConnection();

        return await connection.QuerySingleProcAsync<ChatMessage>("[dbo].[AddMessage]", message, _options.CommandTimeout, cancellationToken);
    }

    public Task<IEnumerable<ChatMessage>> GetLatestMessagesByRoom(string room, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
