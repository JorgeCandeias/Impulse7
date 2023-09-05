//#define VERSION_1
//#define VERSION_2
//#define VERSION_3
//#define VERSION_4
//#define VERSION_5
//#define VERSION_6
//#define VERSION_7
//#define VERSION_8
//#define VERSION_9
//#define VERSION_10
//#define VERSION_11
//#define VERSION_12
#define VERSION_13

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

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _users.Values.AsEnumerable().AsValueTaskResult();
    }
}

#endif

#if VERSION_2

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain
{
    public ActiveChatRoomGrain(ILogger<ActiveChatRoomGrain> logger)
    {
        _logger = logger;
    }

    private readonly ILogger _logger;

    private readonly Queue<ChatMessage> _messages = new();

    private readonly Dictionary<string, ChatUser> _users = new();

    private string _name = "";

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        LogActivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

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

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _users.Values.AsEnumerable().AsValueTaskResult();
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);
}

#endif

#if VERSION_3

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain
{
    public ActiveChatRoomGrain(ILogger<ActiveChatRoomGrain> logger)
    {
        _logger = logger;
    }

    private readonly ILogger _logger;

    private readonly Queue<ChatMessage> _messages = new();

    private readonly Dictionary<string, ChatUser> _users = new();

    private string _name = "";

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        LogActivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

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

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _users.Values.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);
}

#endif

#if VERSION_4

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain
{
    public ActiveChatRoomGrain(ILogger<ActiveChatRoomGrain> logger)
    {
        _logger = logger;
    }

    private readonly ILogger _logger;

    private ImmutableQueue<ChatMessage> _messages = ImmutableQueue<ChatMessage>.Empty;
    private int _count;

    private readonly ImmutableDictionary<string, ChatUser>.Builder _users = ImmutableDictionary.CreateBuilder<string, ChatUser>();

    private string _name = "";

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        LogActivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

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

        _messages = _messages.Enqueue(message);
        _count++;

        if (_count > 10)
        {
            _messages = _messages.Dequeue();
            _count--;
        }

        return Task.CompletedTask;
    }

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _users.ToImmutable().Values.AsValueTaskResult();
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);
}

#endif

#if VERSION_5

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain
{
    public ActiveChatRoomGrain(ILogger<ActiveChatRoomGrain> logger)
    {
        _logger = logger;
    }

    private readonly ILogger _logger;

    private readonly Queue<ChatMessage> _messages = new();

    private readonly Dictionary<string, ChatUser> _users = new();

    private string _name = "";

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        LogActivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

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

        if (_users.Count == 0)
        {
            DeactivateOnIdle();
        }

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

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _users.Values.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        LogDeactivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);

    [LoggerMessage(3, LogLevel.Information, "{GrainType} {Key} deactivated")]
    private partial void LogDeactivated(string grainType, string key);
}

#endif

#if VERSION_6

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain
{
    public ActiveChatRoomGrain(ILogger<ActiveChatRoomGrain> logger, IOptions<ActiveChatRoomOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    private readonly ILogger _logger;
    private readonly ActiveChatRoomOptions _options;
    private readonly Queue<ChatMessage> _messages = new();
    private readonly Dictionary<string, ChatUser> _users = new();
    private string _name = "";

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        LogActivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

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

        if (_users.Count == 0)
        {
            DeactivateOnIdle();
        }

        return Task.CompletedTask;
    }

    public Task Message(ChatMessage message)
    {
        Guard.IsNotNull(message);

        _messages.Enqueue(message);

        if (_messages.Count > _options.MaxCachedMessages)
        {
            _messages.Dequeue();
        }

        return Task.CompletedTask;
    }

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _users.Values.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        LogDeactivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);

    [LoggerMessage(3, LogLevel.Information, "{GrainType} {Key} deactivated")]
    private partial void LogDeactivated(string grainType, string key);
}

#endif

#if VERSION_7

[GenerateSerializer]
internal class ActiveChatRoomGrainState
{
    [Id(1)]
    public Queue<ChatMessage> Messages { get; } = new();

    [Id(2)]
    public Dictionary<string, ChatUser> Users { get; } = new();
}

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain
{
    public ActiveChatRoomGrain(
        ILogger<ActiveChatRoomGrain> logger,
        IOptions<ActiveChatRoomOptions> options,
        [PersistentState("State")] IPersistentState<ActiveChatRoomGrainState> state)
    {
        _logger = logger;
        _options = options.Value;
        _state = state;
    }

    private readonly ILogger _logger;
    private readonly ActiveChatRoomOptions _options;
    private readonly IPersistentState<ActiveChatRoomGrainState> _state;

    private string _name = "";

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        LogActivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

    public async Task Join(ChatUser user)
    {
        Guard.IsNotNull(user);

        _state.State.Users[user.Name] = user;

        await _state.WriteStateAsync();
    }

    public async Task Leave(ChatUser user)
    {
        Guard.IsNotNull(user);

        _state.State.Users.Remove(user.Name);

        await _state.WriteStateAsync();

        if (_state.State.Users.Count == 0)
        {
            DeactivateOnIdle();
        }
    }

    public async Task Message(ChatMessage message)
    {
        Guard.IsNotNull(message);

        _state.State.Messages.Enqueue(message);

        if (_state.State.Messages.Count > _options.MaxCachedMessages)
        {
            _state.State.Messages.Dequeue();
        }

        await _state.WriteStateAsync();
    }

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _state.State.Messages.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _state.State.Users.Values.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        LogDeactivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);

    [LoggerMessage(3, LogLevel.Information, "{GrainType} {Key} deactivated")]
    private partial void LogDeactivated(string grainType, string key);
}

#endif

#if VERSION_8

[GenerateSerializer]
internal class ActiveChatRoomGrainState
{
    [Id(2)]
    public Dictionary<string, ChatUser> Users { get; } = new();
}

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain
{
    public ActiveChatRoomGrain(
        ILogger<ActiveChatRoomGrain> logger,
        IOptions<ActiveChatRoomOptions> options,
        [PersistentState("State")] IPersistentState<ActiveChatRoomGrainState> state,
        IChatRoomRepository roomRepository,
        IChatMessageRepository messageRepository)
    {
        _logger = logger;
        _options = options.Value;
        _state = state;
        _roomRepository = roomRepository;
        _messageRepository = messageRepository;
    }

    private readonly ILogger _logger;
    private readonly ActiveChatRoomOptions _options;
    private readonly IPersistentState<ActiveChatRoomGrainState> _state;
    private readonly IChatRoomRepository _roomRepository;
    private readonly IChatMessageRepository _messageRepository;

    private string _name = "";
    private ChatRoom _room = null!;
    private readonly Queue<ChatMessage> _messages = new();

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        _room = await GrainFactory.GetChatRoomsIndexGrain().GetOrAdd(_name);

        LogActivated(nameof(ActiveChatRoomGrain), _name);
    }

    public async Task Join(ChatUser user)
    {
        Guard.IsNotNull(user);

        _state.State.Users[user.Name] = user;

        await _state.WriteStateAsync();
    }

    public async Task Leave(ChatUser user)
    {
        Guard.IsNotNull(user);

        _state.State.Users.Remove(user.Name);

        await _state.WriteStateAsync();

        if (_state.State.Users.Count == 0)
        {
            DeactivateOnIdle();
        }
    }

    public async Task Message(ChatMessage message)
    {
        Guard.IsNotNull(message);

        var saved = await _messageRepository.Save(message);

        _messages.Enqueue(saved);

        if (_messages.Count > _options.MaxCachedMessages)
        {
            _messages.Dequeue();
        }
    }

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _state.State.Users.Values.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        LogDeactivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);

    [LoggerMessage(3, LogLevel.Information, "{GrainType} {Key} deactivated")]
    private partial void LogDeactivated(string grainType, string key);
}

#endif

#if VERSION_9

[GenerateSerializer]
internal class ActiveChatRoomGrainState
{
    [Id(2)]
    public Dictionary<string, ChatUser> Users { get; } = new();
}

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain
{
    public ActiveChatRoomGrain(
        ILogger<ActiveChatRoomGrain> logger,
        IOptions<ActiveChatRoomOptions> options,
        [PersistentState("State")] IPersistentState<ActiveChatRoomGrainState> state,
        IChatRoomRepository roomRepository,
        IChatMessageRepository messageRepository)
    {
        _logger = logger;
        _options = options.Value;
        _state = state;
        _roomRepository = roomRepository;
        _messageRepository = messageRepository;
    }

    private readonly ILogger _logger;
    private readonly ActiveChatRoomOptions _options;
    private readonly IPersistentState<ActiveChatRoomGrainState> _state;
    private readonly IChatRoomRepository _roomRepository;
    private readonly IChatMessageRepository _messageRepository;

    private string _name = "";
    private ChatRoom _room = null!;
    private readonly Queue<ChatMessage> _messages = new();
    private IAsyncStream<ChatMessage> _stream = null!;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        _room = await GrainFactory.GetChatRoomsIndexGrain().GetOrAdd(_name);

        _stream = this.GetStreamProvider("Chat").GetStream<ChatMessage>(_name);

        LogActivated(nameof(ActiveChatRoomGrain), _name);
    }

    public async Task Join(ChatUser user)
    {
        Guard.IsNotNull(user);

        if (_state.State.Users.TryAdd(user.Name, user))
        {
            await _state.WriteStateAsync();

            await _stream.OnNextAsync(new ChatMessage(Guid.NewGuid(), _name, "System", $"{user.Name} joined chat room {_name}"));
        }
    }

    public async Task Leave(ChatUser user)
    {
        Guard.IsNotNull(user);

        if (_state.State.Users.Remove(user.Name))
        {
            await _state.WriteStateAsync();

            await _stream.OnNextAsync(new ChatMessage(Guid.NewGuid(), _name, "System", $"{user.Name} left chat room {_name}"));
        }

        if (_state.State.Users.Count == 0)
        {
            DeactivateOnIdle();
        }
    }

    public async Task Message(ChatMessage message)
    {
        Guard.IsNotNull(message);

        var saved = await _messageRepository.Save(message);

        _messages.Enqueue(saved);

        if (_messages.Count > _options.MaxCachedMessages)
        {
            _messages.Dequeue();
        }

        await _stream.OnNextAsync(saved);
    }

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _state.State.Users.Values.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        LogDeactivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);

    [LoggerMessage(3, LogLevel.Information, "{GrainType} {Key} deactivated")]
    private partial void LogDeactivated(string grainType, string key);
}

#endif

#if VERSION_10

[GenerateSerializer]
internal class ActiveChatRoomGrainState
{
    [Id(2)]
    public Dictionary<string, ChatUser> Users { get; } = new();
}

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain
{
    public ActiveChatRoomGrain(
        ILogger<ActiveChatRoomGrain> logger,
        IOptions<ActiveChatRoomOptions> options,
        [PersistentState("State")] IPersistentState<ActiveChatRoomGrainState> state,
        IChatRoomRepository roomRepository,
        IChatMessageRepository messageRepository)
    {
        _logger = logger;
        _options = options.Value;
        _state = state;
        _roomRepository = roomRepository;
        _messageRepository = messageRepository;
    }

    private readonly ILogger _logger;
    private readonly ActiveChatRoomOptions _options;
    private readonly IPersistentState<ActiveChatRoomGrainState> _state;
    private readonly IChatRoomRepository _roomRepository;
    private readonly IChatMessageRepository _messageRepository;

    private string _name = "";
    private ChatRoom _room = null!;
    private readonly Queue<ChatMessage> _messages = new();
    private IAsyncStream<ChatMessage> _stream = null!;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        _room = await GrainFactory.GetChatRoomsIndexGrain().GetOrAdd(_name);

        _stream = this.GetStreamProvider("Chat").GetStream<ChatMessage>(_name);

        RegisterTimer(TickLogStats, null!, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));

        LogActivated(nameof(ActiveChatRoomGrain), _name);
    }

    public async Task Join(ChatUser user)
    {
        Guard.IsNotNull(user);

        if (_state.State.Users.TryAdd(user.Name, user))
        {
            await _state.WriteStateAsync();

            await _stream.OnNextAsync(new ChatMessage(Guid.NewGuid(), _name, "System", $"{user.Name} joined chat room {_name}"));
        }
    }

    public async Task Leave(ChatUser user)
    {
        Guard.IsNotNull(user);

        if (_state.State.Users.Remove(user.Name))
        {
            await _state.WriteStateAsync();

            await _stream.OnNextAsync(new ChatMessage(Guid.NewGuid(), _name, "System", $"{user.Name} left chat room {_name}"));
        }

        if (_state.State.Users.Count == 0)
        {
            DeactivateOnIdle();
        }
    }

    public async Task Message(ChatMessage message)
    {
        Guard.IsNotNull(message);

        var saved = await _messageRepository.Save(message);

        _messages.Enqueue(saved);

        if (_messages.Count > _options.MaxCachedMessages)
        {
            _messages.Dequeue();
        }

        await _stream.OnNextAsync(saved);
    }

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _state.State.Users.Values.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        LogDeactivated(nameof(ActiveChatRoomGrain), _name);

        return Task.CompletedTask;
    }

    private Task TickLogStats(object state)
    {
        LogStats(nameof(ActiveChatRoomGrain), _name, _state.State.Users.Count, _messages.Count);

        return Task.CompletedTask;
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);

    [LoggerMessage(3, LogLevel.Information, "{GrainType} {Key} deactivated")]
    private partial void LogDeactivated(string grainType, string key);

    [LoggerMessage(4, LogLevel.Information, "{GrainType} {Key} is active with {UserCount} users and is caching {MessageCount} messages")]
    private partial void LogStats(string grainType, string key, int userCount, int messageCount);
}

#endif

#if VERSION_11

[GenerateSerializer]
internal class ActiveChatRoomGrainState
{
    [Id(2)]
    public Dictionary<string, ChatUser> Users { get; } = new();
}

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain, IRemindable
{
    public ActiveChatRoomGrain(
        ILogger<ActiveChatRoomGrain> logger,
        IOptions<ActiveChatRoomOptions> options,
        [PersistentState("State")] IPersistentState<ActiveChatRoomGrainState> state,
        IChatRoomRepository roomRepository,
        IChatMessageRepository messageRepository)
    {
        _logger = logger;
        _options = options.Value;
        _state = state;
        _roomRepository = roomRepository;
        _messageRepository = messageRepository;
    }

    private readonly ILogger _logger;
    private readonly ActiveChatRoomOptions _options;
    private readonly IPersistentState<ActiveChatRoomGrainState> _state;
    private readonly IChatRoomRepository _roomRepository;
    private readonly IChatMessageRepository _messageRepository;

    private string _name = "";
    private ChatRoom _room = null!;
    private readonly Queue<ChatMessage> _messages = new();
    private IAsyncStream<ChatMessage> _stream = null!;
    private IGrainReminder _reminder = null!;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        _room = await GrainFactory.GetChatRoomsIndexGrain().GetOrAdd(_name);

        _stream = this.GetStreamProvider("Chat").GetStream<ChatMessage>(_name);

        _reminder = await this.RegisterOrUpdateReminder("KeepAlive", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

        RegisterTimer(TickLogStats, null!, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));

        LogActivated(nameof(ActiveChatRoomGrain), _name);
    }

    public async Task Join(ChatUser user)
    {
        Guard.IsNotNull(user);

        if (_state.State.Users.TryAdd(user.Name, user))
        {
            await _state.WriteStateAsync();

            await _stream.OnNextAsync(new ChatMessage(Guid.NewGuid(), _name, "System", $"{user.Name} joined chat room {_name}"));
        }
    }

    public async Task Leave(ChatUser user)
    {
        Guard.IsNotNull(user);

        if (_state.State.Users.Remove(user.Name))
        {
            await _state.WriteStateAsync();

            await _stream.OnNextAsync(new ChatMessage(Guid.NewGuid(), _name, "System", $"{user.Name} left chat room {_name}"));
        }

        if (_state.State.Users.Count == 0)
        {
            DeactivateOnIdle();
        }
    }

    public async Task Message(ChatMessage message)
    {
        Guard.IsNotNull(message);

        var saved = await _messageRepository.Save(message);

        _messages.Enqueue(saved);

        if (_messages.Count > _options.MaxCachedMessages)
        {
            _messages.Dequeue();
        }

        await _stream.OnNextAsync(saved);
    }

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _state.State.Users.Values.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        await this.UnregisterReminder(_reminder);

        LogDeactivated(nameof(ActiveChatRoomGrain), _name);
    }

    private Task TickLogStats(object state)
    {
        LogStats(nameof(ActiveChatRoomGrain), _name, _state.State.Users.Count, _messages.Count);

        return Task.CompletedTask;
    }

    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        switch (reminderName)
        {
            case "KeepAlive":
                LogKeepAlive(nameof(ActiveChatRoomGrain), _name);
                break;

            default:
                break;
        }

        return Task.CompletedTask;
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);

    [LoggerMessage(3, LogLevel.Information, "{GrainType} {Key} deactivated")]
    private partial void LogDeactivated(string grainType, string key);

    [LoggerMessage(4, LogLevel.Information, "{GrainType} {Key} is active with {UserCount} users and is caching {MessageCount} messages")]
    private partial void LogStats(string grainType, string key, int userCount, int messageCount);

    [LoggerMessage(5, LogLevel.Information, "{GrainType} {Key} received keep alive")]
    private partial void LogKeepAlive(string grainType, string key);
}

#endif

#if VERSION_12

[GenerateSerializer]
internal class ActiveChatRoomGrainState
{
    [Id(2)]
    public Dictionary<string, ChatUser> Users { get; } = new();
}

internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain, IRemindable
{
    public ActiveChatRoomGrain(
        ILogger<ActiveChatRoomGrain> logger,
        IOptions<ActiveChatRoomOptions> options,
        [PersistentState("State")] IPersistentState<ActiveChatRoomGrainState> state,
        IChatRoomRepository roomRepository,
        IChatMessageRepository messageRepository)
    {
        _logger = logger;
        _options = options.Value;
        _state = state;
        _roomRepository = roomRepository;
        _messageRepository = messageRepository;
    }

    private readonly ILogger _logger;
    private readonly ActiveChatRoomOptions _options;
    private readonly IPersistentState<ActiveChatRoomGrainState> _state;
    private readonly IChatRoomRepository _roomRepository;
    private readonly IChatMessageRepository _messageRepository;

    private string _name = "";
    private ChatRoom _room = null!;
    private readonly Queue<ChatMessage> _messages = new();
    private IAsyncStream<ChatMessage> _stream = null!;
    private IGrainReminder _reminder = null!;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        _room = await GrainFactory.GetChatRoomsIndexGrain().GetOrAdd(_name);

        _stream = this.GetStreamProvider("Chat").GetStream<ChatMessage>(_name);

        _reminder = await this.RegisterOrUpdateReminder("KeepAlive", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

        RegisterTimer(TickLogStats, null!, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));

        LogActivated(nameof(ActiveChatRoomGrain), _name);
    }

    public async Task Join(ChatUser user)
    {
        Guard.IsNotNull(user);

        if (_state.State.Users.TryAdd(user.Name, user))
        {
            await _state.WriteStateAsync();

            await _stream.OnNextAsync(new ChatMessage(Guid.NewGuid(), _name, "System", $"{user.Name} joined chat room {_name}"));
        }
    }

    public async Task Leave(ChatUser user)
    {
        Guard.IsNotNull(user);

        if (_state.State.Users.Remove(user.Name))
        {
            await _state.WriteStateAsync();

            await _stream.OnNextAsync(new ChatMessage(Guid.NewGuid(), _name, "System", $"{user.Name} left chat room {_name}"));
        }

        if (_state.State.Users.Count == 0)
        {
            DeactivateOnIdle();
        }
    }

    public async Task Message(ChatMessage message)
    {
        Guard.IsNotNull(message);

        LogMessage(nameof(ActiveChatRoomGrain), _name, RequestContext.Get("ActivityId"));

        var saved = await _messageRepository.Save(message);

        _messages.Enqueue(saved);

        if (_messages.Count > _options.MaxCachedMessages)
        {
            _messages.Dequeue();
        }

        await _stream.OnNextAsync(saved);
    }

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _state.State.Users.Values.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        await this.UnregisterReminder(_reminder);

        LogDeactivated(nameof(ActiveChatRoomGrain), _name);
    }

    private Task TickLogStats(object state)
    {
        LogStats(nameof(ActiveChatRoomGrain), _name, _state.State.Users.Count, _messages.Count);

        return Task.CompletedTask;
    }

    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        switch (reminderName)
        {
            case "KeepAlive":
                LogKeepAlive(nameof(ActiveChatRoomGrain), _name);
                break;

            default:
                break;
        }

        return Task.CompletedTask;
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);

    [LoggerMessage(3, LogLevel.Information, "{GrainType} {Key} deactivated")]
    private partial void LogDeactivated(string grainType, string key);

    [LoggerMessage(4, LogLevel.Information, "{GrainType} {Key} is active with {UserCount} users and is caching {MessageCount} messages")]
    private partial void LogStats(string grainType, string key, int userCount, int messageCount);

    [LoggerMessage(5, LogLevel.Information, "{GrainType} {Key} received keep alive")]
    private partial void LogKeepAlive(string grainType, string key);

    [LoggerMessage(6, LogLevel.Information, "{GrainType} {Key} received message from activity {ActivityId}")]
    private partial void LogMessage(string grainType, string key, object activityId);
}

#endif

#if VERSION_13

[GenerateSerializer]
internal class ActiveChatRoomGrainState
{
    [Id(2)]
    public Dictionary<string, ChatUser> Users { get; } = new();
}

[Reentrant]
internal partial class ActiveChatRoomGrain : Grain, IActiveChatRoomGrain, IRemindable
{
    public ActiveChatRoomGrain(
        ILogger<ActiveChatRoomGrain> logger,
        IOptions<ActiveChatRoomOptions> options,
        [PersistentState("State")] IPersistentState<ActiveChatRoomGrainState> state,
        IChatRoomRepository roomRepository,
        IChatMessageRepository messageRepository)
    {
        _logger = logger;
        _options = options.Value;
        _state = state;
        _roomRepository = roomRepository;
        _messageRepository = messageRepository;
    }

    private readonly ILogger _logger;
    private readonly ActiveChatRoomOptions _options;
    private readonly IPersistentState<ActiveChatRoomGrainState> _state;
    private readonly IChatRoomRepository _roomRepository;
    private readonly IChatMessageRepository _messageRepository;

    private string _name = "";
    private ChatRoom _room = null!;
    private readonly Queue<ChatMessage> _messages = new();
    private IAsyncStream<ChatMessage> _stream = null!;
    private IGrainReminder _reminder = null!;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _name = this.GetPrimaryKeyString();

        _room = await GrainFactory.GetChatRoomsIndexGrain().GetOrAdd(_name);

        _stream = this.GetStreamProvider("Chat").GetStream<ChatMessage>(_name);

        _reminder = await this.RegisterOrUpdateReminder("KeepAlive", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

        RegisterTimer(TickLogStats, null!, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));

        LogActivated(nameof(ActiveChatRoomGrain), _name);
    }

    public async Task Join(ChatUser user)
    {
        Guard.IsNotNull(user);

        if (_state.State.Users.TryAdd(user.Name, user))
        {
            await WriteStateAsync();

            await _stream.OnNextAsync(new ChatMessage(Guid.NewGuid(), _name, "System", $"{user.Name} joined chat room {_name}"));
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
        Guard.IsNotNull(message);

        LogMessage(nameof(ActiveChatRoomGrain), _name, RequestContext.Get("ActivityId"));

        var saved = await _messageRepository.Save(message);

        _messages.Enqueue(saved);

        if (_messages.Count > _options.MaxCachedMessages)
        {
            _messages.Dequeue();
        }

        await _stream.OnNextAsync(saved);
    }

    public ValueTask<IEnumerable<ChatMessage>> GetMessages()
    {
        return _messages.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public ValueTask<IEnumerable<ChatUser>> GetUsers()
    {
        return _state.State.Users.Values.ToImmutableArray().AsEnumerable().AsValueTaskResult();
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        await this.UnregisterReminder(_reminder);

        LogDeactivated(nameof(ActiveChatRoomGrain), _name);
    }

    private Task TickLogStats(object state)
    {
        LogStats(nameof(ActiveChatRoomGrain), _name, _state.State.Users.Count, _messages.Count);

        return Task.CompletedTask;
    }

    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        switch (reminderName)
        {
            case "KeepAlive":
                LogKeepAlive(nameof(ActiveChatRoomGrain), _name);
                break;

            default:
                break;
        }

        return Task.CompletedTask;
    }

    [LoggerMessage(2, LogLevel.Information, "{GrainType} {Key} activated")]
    private partial void LogActivated(string grainType, string key);

    [LoggerMessage(3, LogLevel.Information, "{GrainType} {Key} deactivated")]
    private partial void LogDeactivated(string grainType, string key);

    [LoggerMessage(4, LogLevel.Information, "{GrainType} {Key} is active with {UserCount} users and is caching {MessageCount} messages")]
    private partial void LogStats(string grainType, string key, int userCount, int messageCount);

    [LoggerMessage(5, LogLevel.Information, "{GrainType} {Key} received keep alive")]
    private partial void LogKeepAlive(string grainType, string key);

    [LoggerMessage(6, LogLevel.Information, "{GrainType} {Key} received message from activity {ActivityId}")]
    private partial void LogMessage(string grainType, string key, object activityId);

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
}

#endif