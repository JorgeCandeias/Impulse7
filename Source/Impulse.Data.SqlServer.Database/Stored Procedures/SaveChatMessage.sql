CREATE PROCEDURE [dbo].[SaveChatMessage]
	@Guid UNIQUEIDENTIFIER,
	@Room NVARCHAR(100),
	@User NVARCHAR(100),
	@Text NVARCHAR(1000),
	@ETag UNIQUEIDENTIFIER
AS

DECLARE @ChatRoomId INT = (SELECT [Id] FROM [dbo].[ChatRoom] WHERE [Name] = @Room);

DECLARE @ChatUserId INT = (SELECT [Id] FROM [dbo].[ChatUser] WHERE [Name] = @User);

WITH [Source] AS
(
	SELECT
		@Guid AS [Guid],
		@ChatRoomId AS [ChatRoomId],
		@ChatUserId AS [ChatUserId],
		@Text AS [Text],
		@ETag AS [ETag]
)
MERGE INTO [dbo].[ChatMessage] WITH (UPDLOCK, HOLDLOCK) AS [T]
USING [Source] AS [S]
ON [T].[Guid] = [S].[Guid]
WHEN MATCHED AND [T].[ETag] = [S].[ETag] THEN
UPDATE SET
	[Text] = [S].[Text],
	[Updated] = SYSDATETIMEOFFSET(),
	[ETag] = NEWID()
WHEN NOT MATCHED BY TARGET THEN
INSERT
(
	[Guid],
	[ChatRoomId],
	[ChatUserId],
	[Text],
	[Created],
	[Updated],
	[ETag]
)
VALUES
(
	[S].[Guid],
	[S].[ChatRoomId],
	[S].[ChatUserId],
	[S].[Text],
	SYSDATETIMEOFFSET(),
	SYSDATETIMEOFFSET(),
	NEWID()
);

EXECUTE [dbo].[TryGetChatMessageByGuid] @Guid;

GO