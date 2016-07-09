USE HaleDB
GO



PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DummyHosts:   Creating dummy hosts:'
GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DummyHosts:   * Hale Server 1'
EXEC uspCreateHost "Hale Server 1", "halesrv01.cloudapp.net", "127.0.0.1"
GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DummyHosts:   * Hale Server 2'
EXEC uspCreateHost "Hale Server 2", "halesrv02.cloudapp.net", "0.0.0.0"
GO
