CREATE PROCEDURE [dbo].[RemoveChatMessage]
	@Guid UNIQUEIDENTIFIER,
	@ETag UNIQUEIDENTIFIER
AS

DECLARE @Output TABLE ([Id] INT);

DELETE FROM [dbo].[ChatMessage]
OUTPUT [Deleted].[Id]
WHERE
	[Guid] = @Guid
	AND [ETag] = @ETag;

GO