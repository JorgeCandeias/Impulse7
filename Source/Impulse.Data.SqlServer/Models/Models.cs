namespace Impulse.Data.SqlServer.Models;

internal record ChatRoomEntity(int Id, Guid Guid, string Name, DateTimeOffset Created, DateTimeOffset Updated, Guid ETag);

internal record ChatUserEntity(int Id, Guid Guid, string Name, DateTimeOffset Created, DateTimeOffset Updated, Guid ETag);

internal record ChatMessageEntity(int Id, Guid Guid, string Room, string User, string Text, DateTimeOffset Created, DateTimeOffset Updated, Guid ETag);