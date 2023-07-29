CREATE PROCEDURE [dbo].[TryGetChatUserByGuid]
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
	[dbo].[ChatUser]
WHERE
	[Guid] = @Guid

GO