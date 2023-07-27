CREATE PROCEDURE [dbo].[GetAllChatRooms]
AS

SELECT
	[Id],
	[Name],
	[Created]
FROM
	[dbo].[ChatRoom]

GO