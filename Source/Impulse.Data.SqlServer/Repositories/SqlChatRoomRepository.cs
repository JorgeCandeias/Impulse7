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

    public async Task<ChatRoom> GetOrAdd(string name, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNullOrWhiteSpace(name);

        using var connection = Connect();

        var result = await connection.QuerySingleProcAsync<ChatRoomEntity>(
            "[dbo].[GetOrAddChatRoom]",
            new
            {
                name
            },
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<ChatRoom>(result);
    }
}