namespace Impulse.Grains;

public class LoggingGrainCallFilterOptions
{
    public ISet<Assembly> AllowedAssemblies { get; } = new HashSet<Assembly>();
}