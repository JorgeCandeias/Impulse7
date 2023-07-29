namespace Impulse.Data.SqlServer.Models;

internal record ChatRoomEntity(int Id, Guid Guid, string Name, DateTimeOffset Created, DateTimeOffset Updated, Guid ETag);

internal record ChatUserEntity(int Id, string Name, DateTimeOffset Created);