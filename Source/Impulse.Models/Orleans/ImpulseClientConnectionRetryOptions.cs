using System.ComponentModel.DataAnnotations;

namespace Impulse.Models.Orleans;

public class ImpulseClientConnectionRetryOptions
{
    [Required]
    public bool Enabled { get; set; } = true;

    [Required, Range(typeof(TimeSpan), "0.00:00:01.000", "1.00:00:00.000")]
    public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(1);
}