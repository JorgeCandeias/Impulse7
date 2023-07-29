CREATE PROCEDURE [dbo].[SaveChatUser]
	@Guid UNIQUEIDENTIFIER,
	@Name NVARCHAR(100),
	@ETag UNIQUEIDENTIFIER
AS

WITH [Source] AS
(
	SELECT
		@Guid AS [Guid],
		@Name AS [Name],
		@ETag AS [ETag]
)
MERGE INTO [dbo].[ChatUser] WITH (UPDLOCK, HOLDLOCK) AS [T]
USING [Source] AS [S]
ON [T].[Guid] = [S].[Guid]
WHEN MATCHED AND [T].[ETag] = [S].[ETag] THEN
UPDATE SET
	[Name] = [S].[Name],
	[Updated] = SYSDATETIMEOFFSET(),
	[ETag] = NEWID()
WHEN NOT MATCHED BY TARGET THEN
INSERT
(
	[Guid],
	[Name],
	[Created],
	[Updated],
	[ETag]
)
VALUES
(
	[S].[Guid],
	[S].[Name],
	SYSDATETIMEOFFSET(),
	SYSDATETIMEOFFSET(),
	NEWID()
)
OUTPUT
	[Inserted].[Id],
	[Inserted].[Guid],
	[Inserted].[Name],
	[Inserted].[Created],
	[Inserted].[Updated],
	[Inserted].[ETag]
;

GO