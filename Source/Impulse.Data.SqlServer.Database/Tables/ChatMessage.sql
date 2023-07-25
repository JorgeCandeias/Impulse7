﻿CREATE TABLE [dbo].[ChatMessage]
(
	[Id] INT NOT NULL,
	[ChatRoomId] INT NOT NULL,
	[ChatUserId] INT NOT NULL,
	[Text] NVARCHAR(1000) NOT NULL,
	[Created] DATETIMEOFFSET NOT NULL,

	CONSTRAINT [PK_ChatMessage] PRIMARY KEY CLUSTERED
	(
		[Id]
	),

	CONSTRAINT [FK_ChatMessage_ChatRoomId] FOREIGN KEY
	(
		[ChatRoomId]
	)
	REFERENCES [dbo].[ChatRoom]
	(
		[Id]
	),

	CONSTRAINT [FK_ChatMessage_ChatUserId] FOREIGN KEY
	(
		[ChatUserId]
	)
	REFERENCES [dbo].[ChatUser]
	(
		[Id]
	)
)
GO

CREATE NONCLUSTERED INDEX [NCI_ChatMessage_ChatRoomId]
ON [dbo].[ChatMessage]
(
	[ChatRoomId]
)
GO
