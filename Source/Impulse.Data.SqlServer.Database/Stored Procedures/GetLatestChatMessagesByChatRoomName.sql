CREATE PROCEDURE [dbo].[GetLatestChatMessagesByChatRoomName]
	@Name NVARCHAR(100),
	@Count INT
AS

/* this query translates rows to the business model immediately */
SELECT TOP (@Count)
	M.[Guid] AS [Id],
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
	R.[Name] = @Name
ORDER BY
	M.[Created] DESC

GO