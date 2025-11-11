USE [Cmune]
GO

CREATE NONCLUSTERED INDEX [IX_MemberAccess_AccessLevel_Cmid]
ON [dbo].[MemberAccess] ([AccessLevel])
INCLUDE ([Cmid])
GO