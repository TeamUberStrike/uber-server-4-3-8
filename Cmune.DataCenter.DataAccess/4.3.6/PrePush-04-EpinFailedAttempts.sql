USE [Cmune]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EpinFailedAttempts](
	[EpinFailedAttemptId] [int] IDENTITY(1,1) NOT NULL,
	[Cmid] [int] NOT NULL,
	[Ip] [bigint] NOT NULL,
	[Pin] [nvarchar](32) NOT NULL,
	[AttemptDate] [datetime] NOT NULL,
 CONSTRAINT [PK_EpinFailedAttempts] PRIMARY KEY CLUSTERED 
(
	[EpinFailedAttemptId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


