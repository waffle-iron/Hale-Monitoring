USE HaleDB

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables: Dropping tables:'

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.UserDetails'
GO
IF OBJECT_ID('Security.UserDetails', 'U') IS NOT NULL
  DROP TABLE [Security].[UserDetails];

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.Grants'
GO
IF OBJECT_ID('Security.Grants', 'U') IS NOT NULL
DROP TABLE [Security].[Grants]

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.Memberships'
GO
IF OBJECT_ID('Security.Memberships', 'U') IS NOT NULL
DROP TABLE [Security].[Memberships]

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.Permissions'
GO
IF OBJECT_ID('Security.Permissions', 'U') IS NOT NULL
DROP TABLE [Security].[Permissions]

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.Roles'
GO
IF OBJECT_ID('Security.Roles', 'U') IS NOT NULL
DROP TABLE [Security].[Roles]

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Security.Users'
GO
IF OBJECT_ID('Security.Users', 'U') IS NOT NULL
DROP TABLE [Security].[Users]

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Nodes.HostDetails'
GO
IF OBJECT_ID('Nodes.HostDetails', 'U') IS NOT NULL
DROP TABLE [Nodes].[HostDetails]



GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Shared.Metadata'
GO
IF OBJECT_ID('Shared.Metadata', 'U') IS NOT NULL
DROP TABLE [Shared].[Metadata]



GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Checks.Metrics'
GO
IF OBJECT_ID('Checks.Metrics', 'U') IS NOT NULL
  DROP TABLE [Checks].[Metrics];

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Checks.Results'
GO
IF OBJECT_ID('Checks.Results', 'U') IS NOT NULL
  DROP TABLE [Checks].[Results];

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Checks.CheckDetails'
GO
IF OBJECT_ID('Checks.CheckDetails', 'U') IS NOT NULL
  DROP TABLE [Checks].[CheckDetails];

GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Checks.Checks'
GO
IF OBJECT_ID('Checks.Checks', 'U') IS NOT NULL
  DROP TABLE [Checks].[Checks];
    
GO


PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Nodes.Hosts'
GO
IF OBJECT_ID('Nodes.Hosts', 'U') IS NOT NULL
DROP TABLE [Nodes].[Hosts]






PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.Metrics'
GO
IF OBJECT_ID('Modules.Metrics', 'U') IS NOT NULL
DROP TABLE [Modules].[Metrics]

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.InfoData'
GO
IF OBJECT_ID('Modules.InfoData', 'U') IS NOT NULL
DROP TABLE [Modules].[InfoData]

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.Results'
GO
IF OBJECT_ID('Modules.Results', 'U') IS NOT NULL
DROP TABLE [Modules].[Results]

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.Functions'
GO
IF OBJECT_ID('Modules.Functions', 'U') IS NOT NULL
DROP TABLE [Modules].[Functions]

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.FunctionTypes'
GO
IF OBJECT_ID('Modules.FunctionTypes', 'U') IS NOT NULL
DROP TABLE [Modules].[FunctionTypes]

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-Tables:   * Modules.Modules'
GO
IF OBJECT_ID('Modules.Modules', 'U') IS NOT NULL
DROP TABLE [Modules].[Modules]