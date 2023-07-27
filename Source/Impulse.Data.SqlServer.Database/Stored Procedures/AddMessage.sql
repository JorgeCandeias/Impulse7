CREATE PROCEDURE [dbo].[AddMessage]
	@Guid UNIQUEIDENTIFIER,
	@Room NVARCHAR(100),
	@User NVARCHAR(100),
	@Text NVARCHAR(1000),
	@Created DATETIMEOFFSET
AS

DECLARE @ChatRoomId INT;
EXECUTE [dbo].[GetOrAddChatRoomId] @Room, @ChatRoomId OUTPUT;

DECLARE @ChatUserId INT;
EXECUTE [dbo].[GetOrAddChatUserId] @User, @ChatUserId OUTPUT;

DECLARE @Output TABLE ([Id] INT);

INSERT INTO [dbo].[ChatMessage]
(
	[ChatRoomId],
	[ChatUserId],
	[Text],
	[Created]
)
OUTPUT
	[Inserted].[Id]
INTO
	@Output
VALUES
(
	@ChatRoomId,
	@ChatUserId,
	@Text,
	@Created
);

DECLARE @Id INT = (SELECT [Id] FROM @Output);
EXECUTE [dbo].[GetMessageById] @Id;

RETURN 0
GO