USE HaleDB
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   Creating tables:'
GO

--- HaleDB.Security.Users

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.Users'
GO
CREATE TABLE [Security].[Users]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](25) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](88) NOT NULL,
	[Salt] [nvarchar](88) NOT NULL,
	[Encryption] [nvarchar](10) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[Changed] [datetime2](7) NOT NULL,
	[ChangedBy] [int] NOT NULL,
	
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH 
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),
	UNIQUE NONCLUSTERED 
	(
		[Username] ASC
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	)
)

GO



--- HaleDB.Security.Roles

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.Roles'
GO
CREATE TABLE [Security].[Roles]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] nvarchar(100),
	[Value] nvarchar(200),
	[CreatedBy] [int] NOT NULL,
	
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH 
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),
	UNIQUE NONCLUSTERED 
	(
		[Id],
		[Key]
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),
)

GO


--- HaleDB.Security.Memberships

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.Memberships'
GO
CREATE TABLE [Security].[Memberships]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] int NOT NULL,
	[RoleId] int NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH 
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),
	UNIQUE NONCLUSTERED 
	(
		[UserId],
		[RoleId]
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),

	FOREIGN KEY (UserId) REFERENCES [HaleDB].[Security].[Users](Id),
	FOREIGN KEY (RoleId) REFERENCES [HaleDB].[Security].[Roles](Id),
)

GO


--- HaleDB.Security.Permissions

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.Permissions'
GO
CREATE TABLE [Security].[Permissions]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] nvarchar(100),
	[Value] nvarchar(200),
	[CreatedBy] [int] NOT NULL,
	
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH 
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),
	UNIQUE NONCLUSTERED 
	(
		[Id],
		[Key]
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),
)

GO


--- HaleDB.Security.Grants

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.Grants'
GO
CREATE TABLE [Security].[Grants]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] int NOT NULL,
	[PermissionId] int NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH 
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),
	UNIQUE NONCLUSTERED 
	(
	[RoleId],
	[PermissionId]
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),

	FOREIGN KEY ([RoleId]) REFERENCES [HaleDB].[Security].[Roles](Id),
	FOREIGN KEY ([PermissionId]) REFERENCES [HaleDB].[Security].[Permissions](Id),
)

GO


--- HaleDB.Security.UserDetails
GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.UserDetails'
GO
CREATE TABLE [Security].[UserDetails]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Key] [nvarchar](100) NULL,
	[Value] [nvarchar](200) NULL,
	
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)

	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),
	UNIQUE NONCLUSTERED (
		UserId,
		[Key]
		)
)

GO

ALTER TABLE
	[Security].[UserDetails] 
		WITH
		CHECK ADD
			FOREIGN KEY
			(
				[UserId]
			)
	REFERENCES
		[Security].[Users] ([Id]
)


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Nodes.Hosts'
GO
CREATE TABLE [Nodes].[Hosts]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL UNIQUE,
	[HostName] [nvarchar](150) NOT NULL UNIQUE,
	[IP] [nvarchar](15) NULL,
	[LastSeen] [datetime2](7) NULL,
	[Added] [datetime2](7) NOT NULL DEFAULT SYSDATETIME(),
	[Updated] [datetime2](7) NOT NULL DEFAULT SYSDATETIME(),
	[Status] [int] NULL,
	[Guid] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[RsaKey] [varbinary](1024) NULL
	
	
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH 
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),
	UNIQUE NONCLUSTERED
	(
		[Guid] ASC
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	)
	,
)

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Nodes.HostDetails'
GO


CREATE TABLE [Nodes].[HostDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HostId] [int] NOT NULL,
	[Key] [nvarchar](100) NULL,
	[Value] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
UNIQUE NONCLUSTERED 
(
	[HostId] ASC,
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

ALTER TABLE [Nodes].[HostDetails]  WITH CHECK ADD FOREIGN KEY([HostId])
REFERENCES [Nodes].[Hosts] ([Id])
GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Checks.Checks'
GO

CREATE TABLE [Checks].[Checks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Identifier] [varchar](255) NOT NULL

	PRIMARY KEY
	(
		[Id] ASC
	)
)
GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Checks.CheckDetails'
GO

CREATE TABLE [Checks].[CheckDetails] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CheckId] [int] NOT NULL,
	[Version] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](MAX) NULL,
	[Activated] [bit] NOT NULL DEFAULT(0),

	PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),

	UNIQUE NONCLUSTERED
	(
		[CheckId] ASC,
		[Version] ASC	
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
	
	FOREIGN KEY ([CheckId]) REFERENCES [Checks].[Checks](Id)

)


PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Checks.Results'
GO

CREATE TABLE [Checks].[Results](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HostId] [int] NOT NULL,
	[CheckId] [int] NOT NULL,
	[CheckDetailId] [int] NOT NULL,
	[ResultType] [tinyint] NOT NULL,
	[ExecutionTime] [datetime2] NOT NULL,
	[Message] [varchar](MAX),
	[Exception] [varchar](MAX),
	[Target] [varchar](255) NOT NULL DEFAULT 'SELF',

	PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)
	WITH
	(
		PAD_INDEX = OFF
		,STATISTICS_NORECOMPUTE = OFF
		,IGNORE_DUP_KEY = OFF
		,ALLOW_ROW_LOCKS = ON
		,ALLOW_PAGE_LOCKS = ON
	),
	FOREIGN KEY ([CheckId]) REFERENCES [Checks].[Checks](Id),
	FOREIGN KEY ([CheckDetailId]) REFERENCES [Checks].[CheckDetails](Id),
	FOREIGN KEY ([HostId]) REFERENCES [Nodes].[Hosts]([Id]) ON DELETE CASCADE
)

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Checks.Metrics'
GO

CREATE TABLE [Checks].[Metrics] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResultId] [int] NOT NULL,
	[Target] [varchar](255) NOT NULL,
	[RawValue] [float](53) NOT NULL,
	[Weight] [float](24) NOT NULL
)
GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Shared.Metadata'
GO

CREATE TABLE [Shared].[Metadata] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [NVARCHAR](20) NOT NULL,
	[Attribute] [NVARCHAR](20) NOT NULL,
	[Label] [NVARCHAR](30) NOT NULL,
	[Description] [NVARCHAR](200),
	[Required] [BIT] NOT NULL DEFAULT(0),
	[Protected] [BIT] NOT NULL DEFAULT(0)
	
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	),

	UNIQUE NONCLUSTERED (
		[Type] ASC,
		[Attribute] ASC
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	)
)

-- HaleDB.Modules ----------------------------------------------------------------------------------------------------------

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.Functions'
GO
​
CREATE TABLE [Modules].[Functions]
(
	[Id] [int] NOT NULL
	,[Name] [varchar](50) NOT NULL
	,[Type] [tinyint] NOT NULL

	,CONSTRAINT [PK_Functions] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	)
)

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.FunctionTypes'
GO

CREATE TABLE [Modules].[FunctionTypes]
(
	[Id] [tinyint] NOT NULL
	, [Type] [varchar](16) NOT NULL UNIQUE
	
	,CONSTRAINT [PK_FunctionTypes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	)
)

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.Modules'
GO

CREATE TABLE [Modules].[Modules]
(
	[Id] [int] NOT NULL
	,[Identifier] [varchar](255) NULL
	,[Version] [int] NULL
	
	,CONSTRAINT [PK_Modules] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	)

	,CONSTRAINT [UQ_Modules_IdentifierVersion] UNIQUE NONCLUSTERED
	(
		[Identifier] ASC
		, [Version] ASC

	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	)
)

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.Results'
GO

CREATE TABLE [Modules].[Results]
(
	[Id] [int] NOT NULL
	, [HostId] [int] NOT NULL
	, [ModuleId] [int] NOT NULL
	, [FunctionId] [int] NOT NULL
	, [Target] [varchar](50) NULL
	, [ExecutionTime] [datetime2](7) NOT NULL
	, [Message] [varchar](max) NULL
	, [Exception] [varchar](max) NULL
	
	,CONSTRAINT [PK_Results] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	)

	,CONSTRAINT [FK_Results_HostId] FOREIGN KEY
	(
		[HostId]
	)
		REFERENCES
			Nodes.Hosts (Id)
		ON DELETE CASCADE
	
	,CONSTRAINT [FK_Results_ModuleId] FOREIGN KEY
	(
		[ModuleId]
	)
		REFERENCES
			Modules.Modules (Id)
		ON DELETE CASCADE
	
	,CONSTRAINT [FK_Results_FunctionId] FOREIGN KEY
	(
		[FunctionId]
	)
		REFERENCES
			Modules.Functions (Id)
)

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.InfoData'
GO

CREATE TABLE [Modules].[InfoData]
(
	[Id] [int] NOT NULL
	, [ResultId] [int] NOT NULL
	, [Name] [varchar](50) NOT NULL
	, [Target] [varchar](255) NULL
	, [Value] [varchar](255) NOT NULL
	
	,CONSTRAINT [PK_InfoData] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	)
	,CONSTRAINT
		[FK_InfoData_ResultId]
		FOREIGN KEY
			(ResultId)
		REFERENCES
			Modules.Results (Id)
		ON DELETE CASCADE
)

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.Metrics'
GO
​
CREATE TABLE [Modules].[Metrics]
(
	[Id] [int] NOT NULL
	, [ResultId] [int] NOT NULL
	, [Target] [varchar](255) NULL
	, [Name] [varchar](50) NOT NULL
	, [Value] [float] NOT NULL
	
	,CONSTRAINT [PK_Metrics] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	WITH
	(
		PAD_INDEX = OFF
		, STATISTICS_NORECOMPUTE = OFF
		, IGNORE_DUP_KEY = OFF
		, ALLOW_ROW_LOCKS = ON
		, ALLOW_PAGE_LOCKS = ON
	)

	,CONSTRAINT [FK_Metrics_ResultId] FOREIGN KEY
	(
		[ResultId]
	)
	REFERENCES
		Modules.Results
		(
			Id
		)
	ON DELETE CASCADE
)