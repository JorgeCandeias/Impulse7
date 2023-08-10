using Orleans.Placement;
using Orleans.Runtime;
using Orleans.Runtime.Placement;
using System.Collections.Concurrent;

namespace Impulse.Grains.Tests;

[Collection(nameof(TestClusterCollection))]
public class LifetimeGrainExtensionTests
{
    public LifetimeGrainExtensionTests(TestClusterFixture fixture)
    {
        _fixture = fixture;
    }

    private readonly TestClusterFixture _fixture;

    [Fact]
    public async Task Pings()
    {
        // arrange
        var key = Guid.NewGuid();
        var grain = _fixture.TestCluster.Client.GetGrain<IGrainExtensionTestGrain>(key);

        // act
        LifetimeGrainExtensionTestContext.Activated[key] = new();

        await grain.AsReference<ILifetimeGrainExtension>().Ping();

        // assert
        await LifetimeGrainExtensionTestContext.Activated[key].Task.WaitAsync(TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task Deactivates()
    {
        // arrange
        var key = Guid.NewGuid();
        var grain = _fixture.TestCluster.Client.GetGrain<IGrainExtensionTestGrain>(key);

        // act - request deactivation
        LifetimeGrainExtensionTestContext.Activated[key] = new();
        LifetimeGrainExtensionTestContext.Deactivated[key] = new();
        await grain.AsReference<ILifetimeGrainExtension>().DeactivateOnIdle();

        // assert
        await LifetimeGrainExtensionTestContext.Activated[key].Task.WaitAsync(TimeSpan.FromSeconds(10));
        await LifetimeGrainExtensionTestContext.Deactivated[key].Task.WaitAsync(TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task Migrates()
    {
        // arrange
        var key = Guid.NewGuid();
        var grain = _fixture.TestCluster.Client.GetGrain<IGrainExtensionTestGrain>(key);

        // act
        LifetimeGrainExtensionTestContext.Activated[key] = new();
        LifetimeGrainExtensionTestContext.Deactivated[key] = new();
        LifetimeGrainExtensionTestContext.Dehydrated[key] = new();
        LifetimeGrainExtensionTestContext.Rehydrated[key] = new();
        await grain.AsReference<ILifetimeGrainExtension>().MigrateOnIdle();

        // assert
        await LifetimeGrainExtensionTestContext.Activated[key].Task.WaitAsync(TimeSpan.FromSeconds(10));
        await LifetimeGrainExtensionTestContext.Deactivated[key].Task.WaitAsync(TimeSpan.FromSeconds(10));
        await LifetimeGrainExtensionTestContext.Dehydrated[key].Task.WaitAsync(TimeSpan.FromSeconds(10));
        await LifetimeGrainExtensionTestContext.Rehydrated[key].Task.WaitAsync(TimeSpan.FromSeconds(10));
    }
}

public interface ILifetimeGrainExtension : IGrainExtension
{
    ValueTask Ping();

    ValueTask DeactivateOnIdle();

    ValueTask MigrateOnIdle();
}

internal class LifetimeGrainExtension : ILifetimeGrainExtension
{
    public LifetimeGrainExtension(IGrainContext context, ILocalSiloDetails silo)
    {
        _context = context;
        _silo = silo;
    }

    private readonly IGrainContext _context;
    private readonly ILocalSiloDetails _silo;

    public ValueTask Ping()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DeactivateOnIdle()
    {
        _context.Deactivate(new DeactivationReason(DeactivationReasonCode.None, $"Deactivating from {nameof(LifetimeGrainExtension)}"));

        return ValueTask.CompletedTask;
    }

    public ValueTask MigrateOnIdle()
    {
        // arbitrary context provided to the placement director
        var context = new Dictionary<string, object>();

        _context.Migrate(context);

        return ValueTask.CompletedTask;
    }
}

public interface IGrainExtensionTestGrain : IGrainWithGuidKey
{
}

[ForceMigration]
internal class GrainExtensionTestGrain : Grain, IGrainExtensionTestGrain, IGrainMigrationParticipant
{
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var key = this.GetPrimaryKey();

        LifetimeGrainExtensionTestContext.Activated[key].TrySetResult();

        return base.OnActivateAsync(cancellationToken);
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        var key = this.GetPrimaryKey();

        LifetimeGrainExtensionTestContext.Deactivated[key].TrySetResult();

        return base.OnDeactivateAsync(reason, cancellationToken);
    }

    public void OnDehydrate(IDehydrationContext dehydrationContext)
    {
        var key = this.GetPrimaryKey();

        LifetimeGrainExtensionTestContext.Dehydrated[key].TrySetResult();
    }

    public void OnRehydrate(IRehydrationContext rehydrationContext)
    {
        var key = this.GetPrimaryKey();

        LifetimeGrainExtensionTestContext.Rehydrated[key].TrySetResult();
    }
}

internal static class LifetimeGrainExtensionTestContext
{
    public static ConcurrentDictionary<Guid, TaskCompletionSource> Activated = new();

    public static ConcurrentDictionary<Guid, TaskCompletionSource> Deactivated = new();

    public static ConcurrentDictionary<Guid, TaskCompletionSource> Dehydrated = new();

    public static ConcurrentDictionary<Guid, TaskCompletionSource> Rehydrated = new();
}

internal class ForceMigrationPlacementDirector : IPlacementDirector
{
    public ForceMigrationPlacementDirector(ILocalSiloDetails silo)
    {
        _silo = silo;
    }

    private readonly ILocalSiloDetails _silo;

    public Task<SiloAddress> OnAddActivation(PlacementStrategy strategy, PlacementTarget target, IPlacementContext context)
    {
        var compatible = context.GetCompatibleSilos(target);
        var candidates = compatible.Where(x => x != _silo.SiloAddress).ToArray();
        var safe = candidates.Length > 0 ? candidates : compatible;
        var elected = safe[Random.Shared.Next(safe.Length)];

        return Task.FromResult(elected);
    }
}

internal class ForceMigrationStrategy : PlacementStrategy
{
}

internal class ForceMigrationAttribute : PlacementAttribute
{
    public ForceMigrationAttribute() : base(new ForceMigrationStrategy())
    {
    }
}