USE HaleDB


-- [HaleDB].[uspCreateCheck]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-UserStoredProcedures:   * uspCreateCheck'
GO

CREATE PROCEDURE [dbo].[uspCreateCheck]
	@identifier [VARCHAR](255)
	AS
		INSERT INTO [HaleDB].[Checks].[CheckS]
			(
				[Identifier]
			)
			VALUES
			(
				@identifier
			)

-- [HaleDB].[uspUpdateCheck]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspUpdateCheck'
GO

CREATE PROCEDURE [dbo].[uspUpdateCheck]
	@id int
	, @identifier [VARCHAR](255)
	AS
		UPDATE [HaleDB].[Checks].[CheckS]
			SET
				[Identifier] = @identifier
			WHERE [Id] = @id

-- [HaleDB].[uspGetCheck]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspGetCheck'
GO

CREATE PROCEDURE [dbo].[uspGetCheck]
	@id int
	AS
		SELECT
			[Id]
			, [Identifier]
		FROM
			[HaleDB].[Checks].[CheckS]
		WHERE
			[Id] = @id

-- [HaleDB].[uspGetCheckByIdentifier]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspGetCheckByIdentifier'
GO

CREATE PROCEDURE [dbo].[uspGetCheckByIdentifier]
	@identifier int
	AS
		SELECT
			[Id]
			, [Identifier]
		FROM
			[HaleDB].[Checks].[CheckS]
		WHERE
			[Identifier] = @identifier

-- [HaleDB].[uspListChecks]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspListChecks'
GO

CREATE PROCEDURE [dbo].[uspListChecks]
	AS
		SELECT
			[Id]
			, [Identifier]
		FROM
			[HaleDB].[Checks].[CheckS]

-- [HaleDB].[uspDeleteCheck]
GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspDeleteCheck'
GO

CREATE PROCEDURE [dbo].[uspDeleteCheck]
	@id int
	AS
		DELETE
		FROM
			[Checks].[Checks]
		WHERE
			Id = @id

-- [HaleDB].[uspDeleteCheckResult]
GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspDeleteCheckResult'
GO

CREATE PROCEDURE [dbo].[uspDeleteCheckResult]
	@resultId int 
	AS
		DELETE
		FROM
			[Checks].[Results]
		WHERE
			Id = @resultId

-- [HaleDB].[uspCreateCheckResult]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspCreateCheckResult'
GO

CREATE PROCEDURE [dbo].[uspCreateCheckResult]
    -- Locators
	  @hostId int 
	, @checkId int 
	, @checkDetailId int
	-- Add-ons
	, @resultType tinyint
	, @executionTime datetime2
	, @message varchar(max)
	, @exception varchar(max)
	, @target varchar(255)

	AS
		INSERT INTO
			[HaleDB].[Checks].[Results]
		 (
			  [CheckId]
			, [CheckDetailId]
			, [HostId]
			, [ResultType]
			, [ExecutionTime]
			, [Message]
			, [Exception]
			, [Target]
		)
		VALUES
		(
			@checkId
			, @checkDetailId
			, @hostId
			, @resultType
			, @executionTime
			, @message
			, @exception
			, @target
		)


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspUpdateCheckResult'
GO

CREATE PROCEDURE [dbo].[uspUpdateCheckResult]
    -- Locators
	@id int
	, @hostId int 
	, @checkId int 
	, @checkDetailId int
	-- Add-ons
	, @resultType tinyint
	, @executionTime datetime2
	, @message varchar(max)
	, @exception varchar(max)
	, @target varchar(255)

	AS
		UPDATE
			[HaleDB].[Checks].[Results]
		SET
			[HostId] = @hostId
			, [CheckId] = @checkId
			, [CheckDetailId] = @checkDetailId
			, [ResultType] = @resultType
			, [ExecutionTime] = @executionTime
			, [Message] = @message
			, [Exception] = @exception
			, [Target] = @target
		WHERE
			[Id] = @id

-- [HaleDB].[uspGetCheckResult]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspListResultsForHostCheck'
GO

CREATE PROCEDURE [dbo].[uspListResultsForHostCheck]
	@HostId int
	, @CheckId int
	AS
		BEGIN
			SELECT
				[Id]
				, [HostId]
				, [CheckId]
				, [CheckDetailId]
				, [ResultType]
				, [ExecutionTime]
				, [Message]
				, [Exception]
				, [Target]
			FROM
				[HaleDB].[Checks].[Results]
			WHERE
				[HostId] = @HostId
				AND
				[CheckId] = @CheckId
		END

-- [HaleDB].[uspCreateMetric]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspCreateMetric'
GO

CREATE PROCEDURE [dbo].[uspCreateMetric]
	@resultId int
	, @target varchar(255)
	, @rawValue float(53)
	, @weight float(24)
	AS
	BEGIN
		INSERT INTO [HaleDB].[Checks].[Metrics]
			(
				[ResultId]
				, [Target]
				, [RawValue]
				, [Weight]
			)
			VALUES
			(
				@resultId
				, @target
				, @rawValue
				, @weight
			)	
	END


-- [HaleDB].[uspUpdateMetric]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspUpdateMetric'
GO

CREATE PROCEDURE [dbo].[uspUpdateMetric]
	@id int
	, @resultId int
	, @target varchar(255)
	, @rawValue float(53)
	, @weight float(24)
	AS
	BEGIN
		UPDATE [HaleDB].[Checks].[Metrics]
			SET [ResultId] = @resultId
				, [Target] = @target
				, [RawValue] = @rawValue
				, [Weight] = @weight
			WHERE
				[Id] = @id	
	END


-- [HaleDB].[uspDeleteMetric]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspDeleteMetric'
GO

CREATE PROCEDURE [dbo].[uspDeleteMetric]
	@id int
	AS
		BEGIN
			DELETE
			FROM
				[HaleDB].[Checks].[Metrics]
			WHERE
				[Id] = @id
		END

-- [HaleDB].[uspGetMetric]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspGetMetric'
GO

CREATE PROCEDURE [dbo].[uspGetMetric]
	@id int
	AS
		BEGIN
			SELECT
				[Id]
				, [ResultId]
				, [Target]
				, [RawValue]
				, [Weight]
			FROM
				[HaleDB].[Checks].[Metrics]
			WHERE
				[Id] = @id
		END

-- [HaleDB].[uspListMetricsForResult]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspListMetricsForResult'
GO

CREATE PROCEDURE [dbo].[uspListMetricsForResult]
	@resultId int
	AS
		BEGIN
			SELECT
				[Id]
				, [ResultId]
				, [Target]
				, [RawValue]
				, [Weight]
			FROM
				[HaleDB].[Checks].[Metrics]
			WHERE
				[ResultId] = @resultId
		END