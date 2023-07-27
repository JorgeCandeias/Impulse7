CREATE PROCEDURE [dbo].[GetOrAddChatRoomId]
	@Name NVARCHAR(100),
	@Id INT OUTPUT
AS

/* fast path for existing room */
SELECT @Id = [Id]
FROM [dbo].[ChatRoom]
WHERE [Name] = @Name;

IF @Id IS NOT NULL RETURN;

/* slow path for new room */
WITH S AS (SELECT @Name AS [Name], SYSDATETIMEOFFSET() AS [Created])
MERGE INTO [dbo].[ChatRoom] WITH (UPDLOCK, HOLDLOCK) AS T
USING S ON T.[Name] = S.[Name]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Name], [Created])
VALUES ([Name], [Created]);

/* return the new room id */
SELECT @Id = [Id]
FROM [dbo].[ChatRoom]
WHERE [Name] = @Name;

RETURN 0
GO