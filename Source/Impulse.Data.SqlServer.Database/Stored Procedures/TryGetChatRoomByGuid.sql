CREATE PROCEDURE [dbo].[TryGetChatRoomByGuid]
	@Guid UNIQUEIDENTIFIER
AS

SELECT
	[Id],
	[Guid],
	[Name],
	[Created],
	[Updated],
	[ETag]
FROM
	[dbo].[ChatRoom]
WHERE
	[Guid] = @Guid

GO