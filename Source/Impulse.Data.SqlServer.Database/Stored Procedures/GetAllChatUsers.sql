﻿CREATE PROCEDURE [dbo].[GetAllChatUsers]
AS

SELECT
	[Id],
	[Guid],
	[Name],
	[Created],
	[Updated],
	[ETag]
FROM
	[dbo].[ChatUser]

GO