#define VERSION_1
//#define VERSION_10

using Impulse.Core;
using Impulse.Core.Extensions;

namespace Impulse.Grains;

#if VERSION_1

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain
{
    private readonly Queue<ChatMessage> _messages = new();
    private readonly Dictionary<string, ChatUser> _users = new();

    public Task Join(ChatUser user)
    {
        Guard.IsNotNull(user);

        _users[user.Name] = user;

        return Task.CompletedTask;
    }

    public Task Leave(ChatUser user)
    {
        Guard.IsNotNull(user);

        _users.Remove(user.Name);

        return Task.CompletedTask;
    }

    public Task Message(ChatMessage message)
    {
        Guard.IsNotNull(message);

        _messages.Enqueue(message);

        if (_messages.Count > 10)
        {
            _messages.Dequeue();
        }

        return Task.CompletedTask;
    }

    public Task<ImmutableArray<ChatMessage>> GetMessages()
    {
        return _messages.ToImmutableArray().AsTaskResult();
    }

    public Task<ImmutableArray<ChatUser>> GetUsers()
    {
        return _users.Values.ToImmutableArray().AsTaskResult();
    }
}

#endif

#if VERSION_10

[Reentrant]
internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain, IRemindable
{
    private readonly ILogger _logger;
    private readonly ActiveChatRoomOptions _options;
    private readonly IPersistentState<ActiveChatRoomGrainState> _state;

    public ActiveChatRoomGrain(
        ILogger<ActiveChatRoomGrain> logger,
        IOptions<ActiveChatRoomOptions> options,
        [PersistentState("State")] IPersistentState<ActiveChatRoomGrainState> state)
    {
        _logger = logger;
        _options = options.Value;
        _state = state;
    }

    private string _name = null!;

    private IAsyncStream<ChatMessage> _stream = null!;

    private IGrainReminder _reminder = null!;

    private const string GrainType = nameof(ActiveChatRoomGrain);

    public override async Task OnActivateAsync(CancellationToken cancellationToken = default)
    {
        _name = this.GetPrimaryKeyString();

        _stream = this.GetStreamProvider("Chat").GetStream<ChatMessage>(_name);

        RegisterTimer(TickLogStatsTimer, null!, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

        RegisterTimer(TickPublishStatsTimer, null!, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        _reminder = await this.RegisterOrUpdateReminder("Clock", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

        LogActived(GrainType, _name, RequestContext.Get("TraceId"), RequestContext.Get("ClientId"));
    }

    private Task TickLogStatsTimer(object _)
    {
        LogStats(nameof(ActiveChatRoomGrain), _name, _state.State.Users.Count, _state.State.Messages.Count);

        return Task.CompletedTask;
    }

    private Task TickPublishStatsTimer(object _)
    {
        return GrainFactory
            .GetActiveChatRoomLocalStatsGrain()
            .Publish(new ActiveChatRoomStats(_name, _state.State.Users.Count, _state.State.Messages.Count));
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        await this.UnregisterReminder(_reminder);

        LogDeactivated(GrainType, _name, reason);

        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    public async Task Join(ChatUser user)
    {
        Guard.IsNotNull(user);

        if (_state.State.Users.TryAdd(user.Name, user))
        {
            await WriteStateAsync();

            await _stream.OnNextAsync(new ChatMessage(Guid.Empty, _name, "System", $"{user.Name} joined chat room {_name}"));
        }
    }

    public async Task Leave(ChatUser user)
    {
        Guard.IsNotNull(user);

        if (_state.State.Users.Remove(user.Name))
        {
            await WriteStateAsync();

            await _stream.OnNextAsync(new ChatMessage(Guid.NewGuid(), _name, "System", $"{user.Name} left chat room {_name}"));
        }

        if (_state.State.Users.Count == 0)
        {
            DeactivateOnIdle();
        }
    }

    public async Task Message(ChatMessage message)
    {
        // cache the new message
        _state.State.Messages.Enqueue(message);

        // clear any excess messages
        while (_state.State.Messages.Count > _options.MaxCachedMessages)
        {
            _state.State.Messages.Dequeue();
        }

        // persist all messages
        await WriteStateAsync();

        // broad the message to clients
        await _stream.OnNextAsync(message);
    }

    public Task<ImmutableArray<ChatUser>> GetUsers()
    {
        return _state.State.Users.Values.ToImmutableArray().AsTaskResult();
    }

    public Task<ImmutableArray<ChatMessage>> GetMessages()
    {
        return _state.State.Messages.ToImmutableArray().AsTaskResult();
    }

    private Task SendClock()
    {
        return _stream.OnNextAsync(new ChatMessage(
            Guid.NewGuid(),
            _name,
            "System", $"{DateTime.UtcNow:u}: {_name} online with {_state.State.Users.Count} members active"));
    }

    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        switch (reminderName)
        {
            case "Clock":
                return SendClock();

            default:
                break;
        }

        return Task.CompletedTask;
    }

    #region Queued Write

    /// <summary>
    /// Allows state writing to happen in the background.
    /// </summary>
    private Task? _outstandingWriteStateOperation;

    // When reentrant grain is doing WriteStateAsync, etag violations are possible due to concurrent writes.
    // The solution is to serialize and batch writes, and make sure only a single write is outstanding at any moment in time.
    private async Task WriteStateAsync()
    {
        var currentWriteStateOperation = _outstandingWriteStateOperation;
        if (currentWriteStateOperation != null)
        {
            try
            {
                // await the outstanding write, but ignore it since it doesn't include our changes
                await currentWriteStateOperation;
            }
            catch
            {
                // Ignore all errors from this in-flight write operation, since the original caller(s) of it will observe it.
            }
            finally
            {
                if (_outstandingWriteStateOperation == currentWriteStateOperation)
                {
                    // only null out the outstanding operation if it's the same one as the one we awaited, otherwise
                    // another request might have already done so.
                    _outstandingWriteStateOperation = null;
                }
            }
        }

        if (_outstandingWriteStateOperation == null)
        {
            // If after the initial write is completed, no other request initiated a new write operation, do it now.
            currentWriteStateOperation = _state.WriteStateAsync();
            _outstandingWriteStateOperation = currentWriteStateOperation;
        }
        else
        {
            // If there were many requests enqueued to persist state, there is no reason to enqueue a new write
            // operation for each, since any write (after the initial one that we already awaited) will have cumulative
            // changes including the one requested by our caller. Just await the new outstanding write.
            currentWriteStateOperation = _outstandingWriteStateOperation;
        }

        try
        {
            await currentWriteStateOperation;
        }
        finally
        {
            if (_outstandingWriteStateOperation == currentWriteStateOperation)
            {
                // only null out the outstanding operation if it's the same one as the one we awaited, otherwise
                // another request might have already done so.
                _outstandingWriteStateOperation = null;
            }
        }
    }

    #endregion Queued Write

    #region Logging

    [LoggerMessage(1, LogLevel.Information, "{GrainType} {Key} active with {UserCount} users and {MessageCount} cached messages")]
    public partial void LogStats(string grainType, string key, int userCount, int messageCount);

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated with trace {TraceId} from client {ClientId}")]
    public partial void LogActived(string grainType, string key, object traceId, object clientId);

    [LoggerMessage(3, LogLevel.Information, "{GrainType} {Key} deactivated due to {Reason}")]
    public partial void LogDeactivated(string grainType, string key, DeactivationReason reason);

    #endregion Logging
}

#endif

[GenerateSerializer]
internal class ActiveChatRoomGrainState
{
    [Id(1)]
    public Queue<ChatMessage> Messages { get; } = new();

    [Id(2)]
    public Dictionary<string, ChatUser> Users { get; } = new(StringComparer.OrdinalIgnoreCase);
}