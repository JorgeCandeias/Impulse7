namespace Impulse.WebApi.Models;

public record ChatUserCreateRequest(
    [Required] string Name);

public record ChatUserResponse(
    Guid Guid,
    string Name,
    DateTimeOffset Created,
    DateTimeOffset Updated,
    Guid ETag);

public record ChatRoomCreateRequest(
    [Required] string Name);

public record ChatRoomResponse(
    Guid Guid,
    string Name,
    DateTimeOffset Created,
    DateTimeOffset Updated,
    Guid ETag);

public record ChatMessageCreateRequest(
    [Required] string Room,
    [Required] string User,
    [Required] string Text);

public record ChatMessageResponse(
    Guid Guid,
    string Room,
    string User,
    string Text,
    DateTimeOffset Created,
    DateTimeOffset Updated,
    Guid ETag);

public record ActiveChatRoomAddMessageRequest(
    [Required] string Room,
    [Required] string User,
    [Required] string Text);