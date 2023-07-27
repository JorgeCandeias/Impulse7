CREATE PROCEDURE [dbo].[GetOrAddChatUser]
	@Name NVARCHAR(100)
AS

DECLARE @Id INT;
EXECUTE [dbo].[GetOrAddChatUserId] @Name, @Id OUTPUT;

SELECT
	[Id],
	[Name],
	[Created]
FROM
	[dbo].[ChatUser] AS R
WHERE
	R.Id = @Id

RETURN 0
