using Impulse.Core;

namespace Impulse.Grains;

[StatelessWorker(1)]
internal partial class ActiveChatRoomLocalStatsGrain : Grain, IActiveChatRoomLocalStatsGrain
{
    private readonly ILogger _logger;

    public ActiveChatRoomLocalStatsGrain(ILogger<ActiveChatRoomLocalStatsGrain> logger, ILocalSiloDetails silo)
    {
        _logger = logger;
        _siloAddress = silo.SiloAddress;
    }

    private readonly SiloAddress _siloAddress;
    private readonly Dictionary<string, (ActiveChatRoomStats Stats, DateTime Timestamp)> _stats = new();
    private int _users;
    private int _messages;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        RegisterTimer(_ => LogStats(), null!, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        RegisterTimer(_ => Cleanup(), null!, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        RegisterTimer(_ => Push(), null!, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        return Task.CompletedTask;
    }

    public Task Publish(ActiveChatRoomStats stats)
    {
        Guard.IsNotNull(stats, nameof(stats));

        if (_stats.TryGetValue(stats.Name, out var old))
        {
            _users -= old.Stats.UserCount;
            _messages -= old.Stats.MessageCount;
        }

        _users += stats.UserCount;
        _messages += stats.MessageCount;

        _stats[stats.Name] = (stats, DateTime.UtcNow);

        return Task.CompletedTask;
    }

    private Task Cleanup()
    {
        var limit = DateTime.UtcNow.AddSeconds(-10);
        foreach (var item in _stats)
        {
            if (item.Value.Timestamp < limit)
            {
                _stats.Remove(item.Key);
                _users -= item.Value.Stats.UserCount;
                _messages -= item.Value.Stats.MessageCount;

                return Task.CompletedTask;
            }
        }

        return Task.CompletedTask;
    }

    private Task Push()
    {
        return GrainFactory
            .GetActiveChatRoomGlobalStatsGrain()
            .Publish(new ActiveChatRoomSiloStats(_siloAddress, _stats.Count, _users, _messages));
    }

    private Task LogStats()
    {
        LogStats(nameof(ActiveChatRoomGlobalStatsGrain), _users, _messages);

        return Task.CompletedTask;
    }

    [LoggerMessage(1, LogLevel.Information, "{Grain} reports total {Members} members and {Messages} on the local silo")]
    private partial void LogStats(string grain, int members, int messages);
}