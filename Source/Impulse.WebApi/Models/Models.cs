namespace Impulse.WebApi.Models;

public record ChatUserCreateRequest(
    [Required] string Name);

public record ChatUserCreateResponse(
    Guid Guid,
    string Name,
    DateTimeOffset Created,
    DateTimeOffset Updated,
    Guid ETag);

public record ChatRoomCreateRequest(
    [Required] string Name);

public record ChatRoomCreateResponse(
    Guid Guid,
    string Name,
    DateTimeOffset Created,
    DateTimeOffset Updated,
    Guid ETag);