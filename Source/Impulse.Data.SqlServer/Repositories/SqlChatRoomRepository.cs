using AutoMapper;
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

        var input = _mapper.Map<ChatRoomEntity>(room);

        var output = await connection.QuerySingleProcAsync<ChatRoomEntity>(
            "[dbo].[SaveChatRoom]",
            new
            {
                input.Guid,
                input.Name,
                input.ETag
            },
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<ChatRoom>(output);
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
}