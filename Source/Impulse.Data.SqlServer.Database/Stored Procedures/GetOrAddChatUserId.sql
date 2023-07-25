CREATE PROCEDURE [dbo].[GetOrAddChatUserId]
	@Name NVARCHAR(100),
	@Id INT OUTPUT
AS

/* fast path for existing user */
SELECT @Id = [Id]
FROM [dbo].[ChatUser]
WHERE [Name] = @Name;

IF @Id IS NOT NULL RETURN;

/* slow path for new user */
WITH S AS (SELECT @Name AS [Name])
MERGE INTO [dbo].[ChatUser] WITH (XLOCK) AS T
USING S ON T.[Name] = S.[Name]
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name])
VALUES (NEXT VALUE FOR [ChatUserId], @Name);

/* return the new user id */
SELECT @Id = [Id]
FROM [dbo].[ChatUser]
WHERE [Name] = @Name;

RETURN 0
GO