using Impulse.Models;

namespace Impulse.Grains;

[Reentrant]
internal partial class ActiveChatRoomGlobalStatsGrain : Grain, IActiveChatRoomGlobalStatsGrain
{
    private readonly ILogger _logger;

    public ActiveChatRoomGlobalStatsGrain(ILogger<ActiveChatRoomGlobalStatsGrain> logger)
    {
        _logger = logger;
    }

    private readonly Dictionary<SiloAddress, (ActiveChatRoomSiloStats Stats, DateTime Timestamp)> _stats = new();
    private int _rooms;
    private int _users;
    private int _messages;
    private Guid _version = Guid.NewGuid();
    private TaskCompletionSource<(ActiveChatRoomClusterStats Stats, Guid Version)> _completion = new();

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        RegisterTimer(_ => LogStats(), null!, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

        RegisterTimer(_ => Cleanup(), null!, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        return Task.CompletedTask;
    }

    public Task Publish(ActiveChatRoomSiloStats stats)
    {
        Guard.IsNotNull(stats, nameof(stats));

        if (_stats.TryGetValue(stats.SiloAddress, out var old))
        {
            _rooms -= old.Stats.RoomCount;
            _users -= old.Stats.UserCount;
            _messages -= old.Stats.MessageCount;
        }

        _rooms += stats.RoomCount;
        _users += stats.UserCount;
        _messages += stats.MessageCount;

        _stats[stats.SiloAddress] = (stats, DateTime.UtcNow);

        // fullfill pending reactive requests
        _version = Guid.NewGuid();
        _completion.SetResult((new ActiveChatRoomClusterStats(_stats.Count, _rooms, _users, _messages), _version));
        _completion = new();

        return Task.CompletedTask;
    }

    public Task<(ActiveChatRoomClusterStats Stats, Guid Version)> GetStats()
    {
        return Task.FromResult((new ActiveChatRoomClusterStats(_stats.Count, _rooms, _users, _messages), _version));
    }

    [NoProfiling]
    public async Task<(ActiveChatRoomClusterStats Stats, Guid Version)?> WaitForStats(Guid current)
    {
        // resolve the request immediately if the caller has a different version
        if (current != _version)
        {
            return await GetStats();
        }

        // pin the completion to avoid reentrancy issues
        var completion = _completion;

        // wait for the completion to resolve or a timeout
        if (await Task.WhenAny(completion.Task, Task.Delay(TimeSpan.FromSeconds(10))) == completion.Task)
        {
            // fullfill the request with the new data version
            return await completion.Task;
        }
        else
        {
            // this means we dont have anything new
            return null;
        }
    }

    private Task Cleanup()
    {
        var limit = DateTime.UtcNow.AddSeconds(-10);
        foreach (var item in _stats)
        {
            if (item.Value.Timestamp < limit)
            {
                _stats.Remove(item.Key);
                _rooms -= item.Value.Stats.RoomCount;
                _users -= item.Value.Stats.UserCount;
                _messages -= item.Value.Stats.MessageCount;

                return Task.CompletedTask;
            }
        }

        return Task.CompletedTask;
    }

    private Task LogStats()
    {
        LogStats(nameof(ActiveChatRoomGlobalStatsGrain), _users, _messages);

        return Task.CompletedTask;
    }

    [LoggerMessage(1, LogLevel.Information, "{Grain} reports total {Members} members and {Messages} across the cluster")]
    private partial void LogStats(string grain, int members, int messages);
}