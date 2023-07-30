namespace Impulse.WebApi.Models;

public record ApiChatUser(Guid Guid, string Name, DateTimeOffset Created, DateTimeOffset Updated, Guid ETag);