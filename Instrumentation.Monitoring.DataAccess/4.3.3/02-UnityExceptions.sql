USE [CmuneMonitoring]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UnityExceptions]') AND type in (N'U'))
DROP TABLE [dbo].[UnityExceptions]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UnityExceptions](
	[UnityExceptionId] [int] IDENTITY(1,1) NOT NULL,
	[ExceptionType] [nvarchar](50) NOT NULL,
	[ExceptionMessage] [nvarchar](200) NOT NULL,
	[Cmid] [int] NOT NULL,
	[Channel] [int] NOT NULL,
	[Build] [int] NOT NULL,
	[ExceptionTime] [datetime] NOT NULL,
	[Stacktrace] [nvarchar](max) NOT NULL,
	[StacktraceHash] [nvarchar](100) NOT NULL,
	[FaultiveFunction] [nvarchar](100) NOT NULL,
	[ExceptionData] [nvarchar](max) NOT NULL,
	[BuildNumber] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_UnityExceptions] PRIMARY KEY CLUSTERED 
(
	[UnityExceptionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO