using Impulse.Models;

namespace Impulse.Grains;

[Reentrant]
[StatelessWorker(1)]
internal class ActiveChatRoomGlobalStatsReplicaGrain : Grain, IActiveChatRoomGlobalStatsReplicaGrain
{
    private ActiveChatRoomClusterStats _stats = null!;
    private Guid _version;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        (_stats, _version) = await GrainFactory.GetActiveChatRoomGlobalStatsGrain().GetStats();

        RegisterTimer(async _ =>
        {
            var result = await GrainFactory.GetActiveChatRoomGlobalStatsGrain().WaitForStats(_version);
            if (result.HasValue)
            {
                (_stats, _version) = result.Value;
            }
        }, null!, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(1));
    }

    public Task<ActiveChatRoomClusterStats> GetStats()
    {
        return Task.FromResult(_stats);
    }
}