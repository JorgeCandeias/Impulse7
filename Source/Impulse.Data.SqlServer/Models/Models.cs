namespace Impulse.Data.SqlServer.Models;

internal record ChatRoomEntity(int Id, string Name, DateTimeOffset Created);

internal record ChatUserEntity(int Id, string Name, DateTimeOffset Created);