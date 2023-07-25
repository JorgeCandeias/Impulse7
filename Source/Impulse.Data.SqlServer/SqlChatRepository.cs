using Dapper;
using Impulse.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Impulse.Data.SqlServer;

internal class SqlChatRepository : IChatRepository
{
    private readonly SqlRepositoryOptions _options;

    public SqlChatRepository(IOptions<SqlRepositoryOptions> options)
    {
        _options = options.Value;
    }

    public Task<ChatMessage> AddMessage(ChatMessage message)
    {
        using var connection = new SqlConnection(_options.ConnectionString);

        connection.ExecuteScalar<int>()
    }

    public Task<ChatMessage> GetMessage(int id)
    {
        throw new NotImplementedException();
    }
}
