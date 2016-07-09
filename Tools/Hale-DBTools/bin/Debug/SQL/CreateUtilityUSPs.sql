USE HaleDB


/*

	Serializing:
		PRINT usfSerializeVersion(2, 2, 2)


	Deserializing:
		SELECT
			*
			,(SELECT Major FROM usfDeserializeVersion(Id))  AS Major
			,(SELECT Minor FROM usfDeserializeVersion(Id))  AS Minor
			,(SELECT Revision FROM usfDeserializeVersion(Id))  AS Revision
		FROM
			Checks.Checks

*/ 

-- [HaleDB].[uspSerializeVersion]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-UtilityFunctions:   * ufnSerializeVersion'
GO

CREATE FUNCTION [dbo].[ufnSerializeVersion]
	(
		@Major int = 0,
		@Minor int = 0,
		@Revision int = 0
	)

	RETURNS int
	AS
	BEGIN
		DECLARE @Version int
		SET @Version = @Revision + @Minor*POWER(2, 15) + @Major*POWER(2,23);
		RETURN @Version
	END

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-UtilityFunctions:   * ufnDeserializeVersion'
GO

CREATE FUNCTION [dbo].[ufnDeserializeVersion]
	(
		@Version int
	)

	RETURNS @Versions TABLE
	(
		Major int NOT NULL
		, Minor int NOT NULL
		, Revision int NOT NULL
	)
	AS
	BEGIN
		DECLARE
			@Major int,
			@Minor int,
			@Revision int;
		
		SELECT
			@Major = @Version / POWER(2,23),
			@Minor = (@Version / POWER(2, 15)) % POWER(2,8),
			@Revision = @Version % POWER(2, 15);
		INSERT @Versions
		SELECT @Major, @Minor, @Revision
		RETURN;
	END