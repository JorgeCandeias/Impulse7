using Impulse.Core.Exceptions;
using Impulse.Data.SqlServer.Models;

namespace Impulse.Data.SqlServer.Repositories;

internal class SqlChatRoomRepository : IChatRoomRepository
{
    public SqlChatRoomRepository(IOptions<SqlRepositoryOptions> options, IMapper mapper)
    {
        _options = options.Value;
        _mapper = mapper;
    }

    private readonly SqlRepositoryOptions _options;
    private readonly IMapper _mapper;

    private SqlConnection Connect() => new(_options.ConnectionString);

    public async Task<ChatRoom> Save(ChatRoom room, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(room);

        using var connection = Connect();

        var entity = _mapper.Map<ChatRoomEntity>(room);

        var result = await connection.QuerySingleProcAsync<ChatRoomEntity>(
            "[dbo].[SaveChatRoom]",
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

            throw new ConflictException(entity.ETag, etag);
        }

        return _mapper.Map<ChatRoom>(result);
    }

    public async Task<Guid?> TryGetETagByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        return await connection.ExecuteScalarProcAsync<Guid?>(
            "[dbo].[TryGetChatRoomETagByGuid]",
            new
            {
                guid
            },
            _options.CommandTimeout,
            cancellationToken);
    }

    public async Task<ChatRoom?> TryGetByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        var result = await connection.QuerySingleOrDefaultProcAsync<ChatRoomEntity>(
            "[dbo].[TryGetChatRoomByGuid]",
            new
            {
                guid
            },
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<ChatRoom>(result);
    }

    public async Task<ChatRoom?> TryGetByName(string name, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(name);

        using var connection = Connect();

        var result = await connection.QuerySingleOrDefaultProcAsync<ChatRoomEntity>(
            "[dbo].[TryGetChatRoomByName]",
            new
            {
                name
            },
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<ChatRoom>(result);
    }

    public async Task<IEnumerable<ChatRoom>> GetAll(CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        var result = await connection.QueryProcAsync<ChatRoomEntity>(
            "[dbo].[GetAllChatRooms]",
            null,
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<IEnumerable<ChatRoom>>(result);
    }

    public async Task Remove(Guid guid, Guid etag, CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        var result = await connection.QuerySingleOrDefaultProcAsync<ChatRoomEntity>(
            "[dbo].[RemoveChatRoom]",
            new
            {
                guid,
                etag
            },
            _options.CommandTimeout,
            cancellationToken);

        if (result is null)
        {
            var storedETag = await TryGetETagByGuid(guid, cancellationToken);

            throw new ConflictException(etag, storedETag);
        }
    }
}