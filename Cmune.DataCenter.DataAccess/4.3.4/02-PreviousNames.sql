USE [Cmune]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PreviousNames_Members]') AND parent_object_id = OBJECT_ID(N'[dbo].[PreviousNames]'))
ALTER TABLE [dbo].[PreviousNames] DROP CONSTRAINT [FK_PreviousNames_Members]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PreviousNames]') AND type in (N'U'))
DROP TABLE [dbo].[PreviousNames]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PreviousNames](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Cmid] [int] NOT NULL,
	[PreviousUserName] [nvarchar](50) NOT NULL,
	[ChangeDate] [datetime] NOT NULL,
	[SourceIp] [bigint] NOT NULL,
 CONSTRAINT [PK_PreviousNames] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PreviousNames]  WITH CHECK ADD  CONSTRAINT [FK_PreviousNames_Members] FOREIGN KEY([Cmid])
REFERENCES [dbo].[Members] ([CMID])
GO

ALTER TABLE [dbo].[PreviousNames] CHECK CONSTRAINT [FK_PreviousNames_Members]
GO