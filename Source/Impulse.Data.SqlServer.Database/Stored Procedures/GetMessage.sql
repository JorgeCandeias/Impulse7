CREATE PROCEDURE [dbo].[GetMessage]
	@Id INT
AS

SELECT
	M.[Id],
	U.[Name] AS [User],
	M.[Text],
	M.[Created]
FROM
	[dbo].[ChatMessage] AS M
	INNER JOIN [dbo].[ChatUser] AS U
		ON U.[Id] = M.[ChatUserId]
WHERE
	M.[Id] = @Id

RETURN 0
