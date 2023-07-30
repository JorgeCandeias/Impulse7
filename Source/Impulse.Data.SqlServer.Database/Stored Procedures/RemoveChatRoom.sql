CREATE PROCEDURE [dbo].[RemoveChatRoom]
	@Guid UNIQUEIDENTIFIER,
	@ETag UNIQUEIDENTIFIER
AS

DELETE FROM [dbo].[ChatRoom]
OUTPUT
	[Deleted].[Id],
	[Deleted].[Guid],
	[Deleted].[Name],
	[Deleted].[Created],
	[Deleted].[Updated],
	[Deleted].[ETag]
WHERE
	[Guid] = @Guid
	AND [ETag] = @ETag;

GO