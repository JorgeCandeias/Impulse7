CREATE PROCEDURE [dbo].[AddMessage]
	@ChatRoomName NVARCHAR(100),
	@ChatUserName NVARCHAR(100),
	@Text NVARCHAR(1000),
	@Created DATETIMEOFFSET
AS

DECLARE @ChatRoomId INT;
EXECUTE [dbo].[GetOrAddChatRoomId] @ChatRoomName, @ChatRoomId OUTPUT;

DECLARE @ChatUserId INT;
EXECUTE [dbo].[GetOrAddChatUserId] @ChatUserName, @ChatUserId OUTPUT;

DECLARE @Id INT = NEXT VALUE FOR [ChatMessageId];

INSERT INTO [dbo].[ChatMessage]
(
	[Id],
	[ChatRoomId],
	[ChatUserId],
	[Text],
	[Created]
)
VALUES
(
	@Id,
	@ChatRoomId,
	@ChatUserId,
	@Text,
	@Created
);

EXECUTE [dbo].[GetMessage] @Id = @Id;

RETURN 0
GO