CREATE PROCEDURE [dbo].[GetMessageById]
	@Id INT
AS

SELECT
	M.[Guid],
	R.[Name] AS [Room],
	U.[Name] AS [User],
	M.[Text],
	M.[Created]
FROM
	[dbo].[ChatMessage] AS M
	INNER JOIN [dbo].[ChatRoom] AS R
		ON R.[Id] = M.[ChatRoomId]
	INNER JOIN [dbo].[ChatUser] AS U
		ON U.[Id] = M.[ChatUserId]
WHERE
	M.[Id] = @Id

GO