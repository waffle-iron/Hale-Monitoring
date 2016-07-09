USE HaleDB
GO


CREATE TABLE #uspDropList (
  [Id] int identity(1,1),
  [Name] nvarchar(50)
)

GO

-- Drop Array
INSERT INTO #uspDropList (Name) VALUES
	-- Utility Procedures
	  ('ufnSerializeVersion')
	, ('ufnDeserializeVersion')
GO



-- Dropping Method

DECLARE @position int;
DECLARE @currentName nvarchar(50)
DECLARE @dropSql nvarchar(500)
SET @position = (SELECT MIN(id) FROM  #uspDropList);
	
WHILE (@position <= (SELECT MAX(id) FROM #uspDropList))
BEGIN
	SET @currentName = (SELECT Name FROM #uspDropList WHERE Id = @position);
	IF EXISTS
		(SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = @currentName AND ROUTINE_SCHEMA = 'dbo' AND ROUTINE_TYPE = 'PROCEDURE')
	BEGIN
		SET @dropSql = 'DROP PROCEDURE ' + @currentName;
		EXEC(@dropSql);
		PRINT CAST(SYSDATETIME() AS NVARCHAR(19)) + ' - Dropped stored functions ' + @currentName + ' from the database.'
	END
	SET @position = (@position + 1)
END

DROP TABLE #uspDropList

