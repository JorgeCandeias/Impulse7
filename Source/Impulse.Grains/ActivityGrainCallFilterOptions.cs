namespace Impulse.Grains;

public class ActivityGrainCallFilterOptions
{
    public ISet<Assembly> AllowedAssemblies { get; } = new HashSet<Assembly>();
}