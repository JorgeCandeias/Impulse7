﻿CREATE PROCEDURE [dbo].[GetAllChatRooms]
AS

SELECT
	[Id],
	[Guid],
	[Name],
	[Created],
	[Updated],
	[ETag]
FROM
	[dbo].[ChatRoom]

GO