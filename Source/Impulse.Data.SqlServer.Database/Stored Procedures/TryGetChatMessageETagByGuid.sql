CREATE PROCEDURE [dbo].[TryGetChatMessageETagByGuid]
	@Guid UNIQUEIDENTIFIER
AS

SELECT
	[ETag]
FROM
	[dbo].[ChatMessage]
WHERE
	[Guid] = @Guid

GO