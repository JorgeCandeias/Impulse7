CREATE PROCEDURE [dbo].[TryGetChatRoomByName]
	@Name NVARCHAR(100)
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
	[Name] = @Name

GO