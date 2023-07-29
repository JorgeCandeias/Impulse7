using Impulse.Data.SqlServer.Models;

namespace Impulse.Data.SqlServer.Repositories;

internal class SqlChatMessageRepository : IChatMessageRepository
{
    public SqlChatMessageRepository(IOptions<SqlRepositoryOptions> options, IMapper mapper)
    {
        _options = options.Value;
        _mapper = mapper;
    }

    private readonly SqlRepositoryOptions _options;
    private readonly IMapper _mapper;

    private SqlConnection Connect() => new(_options.ConnectionString);

    public async Task<ChatMessage> Save(ChatMessage message, CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        var entity = _mapper.Map<ChatMessageEntity>(message);

        var result = await connection.QuerySingleOrDefaultProcAsync<ChatMessageEntity>(
            "[dbo].[SaveChatMessage]",
            new
            {
                entity.Guid,
                entity.Room,
                entity.User,
                entity.Text,
                entity.ETag
            },
            _options.CommandTimeout,
            cancellationToken);

        if (result is null)
        {
            var etag = await TryGetETagByGuid(entity.Guid, cancellationToken);

            throw new InconsistentStateException("Inconsistent state detected while saving chat message", etag.ToString(), entity.ETag.ToString());
        }

        return _mapper.Map<ChatMessage>(result);
    }

    public async Task<Guid?> TryGetETagByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        return await connection.ExecuteScalarProcAsync<Guid?>(
            "[dbo].[TryGetChatMessageETagByGuid]",
            new
            {
                guid
            },
            _options.CommandTimeout,
            cancellationToken);
    }

    public async Task<ChatMessage?> TryGetByGuid(Guid guid, CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        var result = await connection.QuerySingleOrDefaultProcAsync<ChatMessageEntity>(
            "[dbo].[TryGetChatMessageByGuid]",
            new
            {
                guid
            },
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<ChatMessage>(result);
    }

    public async Task<IEnumerable<ChatMessage>> GetAll(CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        var result = await connection.QueryProcAsync<ChatMessageEntity>(
            "[dbo].[GetAllMessages]",
            null,
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<IEnumerable<ChatMessage>>(result);
    }

    public async Task<IEnumerable<ChatMessage>> GetLatestCreatedByRoom(string name, int count, CancellationToken cancellationToken = default)
    {
        using var connection = Connect();

        var result = await connection.QueryProcAsync<ChatMessageEntity>(
            "[dbo].[GetLatestChatMessagesByChatRoomName]",
            new
            {
                name,
                count
            },
            _options.CommandTimeout,
            cancellationToken);

        return _mapper.Map<IEnumerable<ChatMessage>>(result);
    }
}