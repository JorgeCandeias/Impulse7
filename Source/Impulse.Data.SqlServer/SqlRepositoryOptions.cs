using System.ComponentModel.DataAnnotations;

namespace Impulse.Data.SqlServer;

public class SqlRepositoryOptions
{
    [Required]
    public string ConnectionString { get; set; } = "";

    [Required]
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(20);
}
