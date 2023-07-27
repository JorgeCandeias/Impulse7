using AutoMapper;
using Impulse.Data.SqlServer.Models;

namespace Impulse.Data.SqlServer.Repositories;

internal class SqlChatUserRepository : IChatUserRepository
{
    public SqlChatUserRepository(IOptions<SqlRepositoryOptions> options, IMapper mapper)
    {
        _options = options.Value;
        _mapper = mapper;
    }

    private readonly SqlRepositoryOptions _options;
    private readonly IMapper _mapper;

    private SqlConnection Connect() => new(_options.ConnectionString);

    public async Task<IEnumerable<ChatUser>> GetAll(CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        var result = await connection.QueryProcAsync<ChatUserEntity>(
            "[dbo].[GetAllChatUsers]",
            null,
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<IEnumerable<ChatUser>>(result);
    }

    public async Task<ChatUser> GetOrAdd(string name, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var connection = Connect();

        var result = await connection.QuerySingleProcAsync<ChatUserEntity>(
            "[dbo].[GetOrAddChatUser]",
            new
            {
                name
            },
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<ChatUser>(result);
    }
}