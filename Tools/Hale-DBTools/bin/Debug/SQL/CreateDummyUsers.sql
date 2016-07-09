USE HaleDB
GO


PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DummyUsers:   Creating dummy users:'
GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DummyUsers:   * nils'
EXEC uspAddUser "nils", "nils@nils.nils", "MGlG2SmWk4Bh78szqSnzMsbATRBDH+rwI8q/egA6v8lIbbspVJ3B6umi2bkTI/DoDSgMXezUSGxXkkWSuVOxeQ==", "cadb691f-7f94-41f3-a504-7067e7021437", "SHA512", 0
GO

PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DummyUsers:   * simon'
EXEC uspAddUser "simon", "simon@simon.simon", "S3hAVnNcJmN3r//DFtnhdEcUcoaLNdWr3i51pV+mZVvf6DGAV5LnrZQQ34bR2qffJH7fMz3I/PhOhQgOnHogPQ==", "2f9e4d7d-fedf-459a-bf07-4bb34027911a", "SHA512", 0
GO
