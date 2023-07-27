CREATE PROCEDURE [dbo].[GetOrAddChatRoom]
	@Name NVARCHAR(100)
AS

DECLARE @Id INT;
EXECUTE [dbo].[GetOrAddChatRoomId] @Name, @Id OUTPUT;

SELECT
	[Id],
	[Name],
	[Created]
FROM
	[dbo].[ChatRoom] AS R
WHERE
	R.Id = @Id

RETURN 0
