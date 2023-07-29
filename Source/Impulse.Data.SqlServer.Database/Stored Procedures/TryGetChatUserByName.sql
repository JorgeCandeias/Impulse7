CREATE PROCEDURE [dbo].[TryGetChatUserByName]
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
	[dbo].[ChatUser]
WHERE
	[Name] = @Name

GO