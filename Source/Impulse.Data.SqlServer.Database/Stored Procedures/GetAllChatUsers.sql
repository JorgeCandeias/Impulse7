CREATE PROCEDURE [dbo].[GetAllChatUsers]
AS

SELECT
	[Id],
	[Name],
	[Created]
FROM
	[dbo].[ChatUser]

GO