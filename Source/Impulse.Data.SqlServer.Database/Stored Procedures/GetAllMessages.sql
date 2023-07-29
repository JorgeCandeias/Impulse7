CREATE PROCEDURE [dbo].[GetAllMessages]
AS

SELECT
	[M].[Id],
	[M].[Guid],
	[R].[Name] AS [Room],
	[U].[Name] AS [User],
	[M].[Text],
	[M].[Created],
	[M].[Updated],
	[M].[ETag]
FROM
	[dbo].[ChatMessage] AS [M]
	INNER JOIN [dbo].[ChatRoom] AS [R]
		ON [R].[Id] = [M].[ChatRoomId]
	INNER JOIN [dbo].[ChatUser] AS [U]
		ON [U].[Id] = [M].[ChatUserId]

GO