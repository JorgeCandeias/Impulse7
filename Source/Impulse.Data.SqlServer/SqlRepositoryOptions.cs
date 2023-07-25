using System.ComponentModel.DataAnnotations;

namespace Impulse.Data.SqlServer;

public class SqlRepositoryOptions
{
    [Required]
    public string ConnectionString { get; set; } = "";
}
