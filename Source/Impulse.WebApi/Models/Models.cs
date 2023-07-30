using System.ComponentModel.DataAnnotations;

namespace Impulse.WebApi.Models;

public record ChatUserCreateRequest(
    [Required] string Name);

public record ChatUserCreateResponse(
    Guid Guid,
    string Name,
    DateTimeOffset Created,
    DateTimeOffset Updated,
    Guid ETag);