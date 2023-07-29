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

    public async Task<ChatUser> Save(ChatUser room, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(room);

        using var connection = Connect();

        var input = _mapper.Map<ChatUserEntity>(room);

        var output = await connection.QuerySingleProcAsync<ChatUserEntity>(
            "[dbo].[SaveChatUser]",
            new
            {
                input.Guid,
                input.Name,
                input.ETag
            },
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<ChatUser>(output);
    }

    public async Task<ChatUser?> TryGetByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        var result = await connection.QuerySingleOrDefaultProcAsync<ChatUserEntity>(
            "[dbo].[TryGetChatUserByGuid]",
            new
            {
                guid
            },
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<ChatUser>(result);
    }

    public async Task<ChatUser?> TryGetByName(string name, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(name);

        using var connection = Connect();

        var result = await connection.QuerySingleOrDefaultProcAsync<ChatUserEntity>(
            "[dbo].[TryGetChatUserByName]",
            new
            {
                name
            },
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<ChatUser>(result);
    }

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
}