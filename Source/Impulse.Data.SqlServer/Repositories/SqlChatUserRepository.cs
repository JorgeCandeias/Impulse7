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

        var entity = _mapper.Map<ChatUserEntity>(room);

        var result = await connection.QuerySingleProcAsync<ChatUserEntity>(
            "[dbo].[SaveChatUser]",
            new
            {
                entity.Guid,
                entity.Name,
                entity.ETag
            },
            _options.CommandTimeout,
            cancellationToken);

        if (result is null)
        {
            var etag = await TryGetETagByGuid(entity.Guid, cancellationToken);

            throw new InconsistentStateException("Inconsistent state detected while saving chat user", etag.ToString(), entity.ETag.ToString());
        }

        return _mapper.Map<ChatUser>(result);
    }

    public async Task<Guid?> TryGetETagByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        return await connection.ExecuteScalarProcAsync<Guid?>(
            "[dbo].[TryGetChatUserETagByGuid]",
            new
            {
                guid
            },
            _options.CommandTimeout,
            cancellationToken);
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