USE [GenesisChallenge]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 01/05/2018 12:23:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[userId] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[email] [nvarchar](100) NOT NULL,
	[passwordHash] [nvarchar](max) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[lastUpdatedOn] [datetime] NULL,
	[lastLoginOn] [datetime] NOT NULL,
	[token] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

/****** Object:  Table [dbo].[UsersPhones]    Script Date: 01/05/2018 12:23:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[UsersPhones](
	[userPhoneId] [bigint] IDENTITY(1,1) NOT NULL,
	[userId] [uniqueidentifier] NOT NULL,
	[phone] [varchar](15) NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


