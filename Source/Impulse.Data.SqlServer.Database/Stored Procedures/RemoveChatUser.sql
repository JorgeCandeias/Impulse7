CREATE PROCEDURE [dbo].[RemoveChatUser]
	@Guid UNIQUEIDENTIFIER,
	@ETag UNIQUEIDENTIFIER
AS

DELETE FROM [dbo].[ChatUser]
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