﻿CREATE TABLE [dbo].[ChatUser]
(
	[Id] INT NOT NULL IDENTITY (1, 1),
	[Guid] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[Created] DATETIMEOFFSET NOT NULL,
	[Updated] DATETIMEOFFSET NOT NULL,
	[ETag] UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT [PK_ChatUser] PRIMARY KEY CLUSTERED
	(
		[Id]
	),

	CONSTRAINT [UK_ChatUser_Guid] UNIQUE NONCLUSTERED
	(
		[Guid]
	),

	CONSTRAINT [UK_ChatUser_Name] UNIQUE NONCLUSTERED
	(
		[Name]
	)
)
GO