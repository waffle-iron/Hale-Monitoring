USE HaleDB


-- [HaleDB].[uspGetMetadata]


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-UserStoredProcedures:   * uspGetMetadata'
GO

CREATE PROCEDURE [dbo].[uspListMetadata]
	@type nvarchar(20)
	, @attribute nvarchar(20)
	AS
		SELECT
			[Id]
			,[Type]
			,[Attribute]
			,[Label]
			,[Description]
			,[Required]
			,[Protected]

		FROM
			[HaleDB].[Shared].[Metadata]
		WHERE
			[Type] = @type
			AND
			[Attribute] = @attribute

-- [HaleDB].[uspListMetadataByType]


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-UserStoredProcedures:   * uspListMetadataByType'
GO

CREATE PROCEDURE [dbo].[uspListMetadataByType]
	@type nvarchar(20)
	AS
		SELECT
			[Id]
			,[Type]
			,[Attribute]
			,[Label]
			,[Description]
			,[Required]
			,[Protected]

		FROM
			[HaleDB].[Shared].[Metadata]
		WHERE
			[Type] = @type


-- [HaleDB].[uspCreateMetadata]


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-UserStoredProcedures:   * uspAddMetadataAttribute'
GO

CREATE PROCEDURE [dbo].[uspCreateMetadata]
	@type nvarchar(20)
	, @attribute nvarchar(20)
	, @label nvarchar(30)
	, @description nvarchar(200)
	, @required bit
	, @protected bit
	AS
		INSERT INTO
			[HaleDB].[Shared].[Metadata]
		 (
			[Type]
			,[Attribute]
			,[Label]
			,[Description]
			,[Required]
			,[Protected]
		)
		VALUES
		(
			@type
			, @attribute
			, @label
			, @description
			, @required
			, @protected
		)

-- [HaleDB].[uspUpdateMetadata]


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-UserStoredProcedures:   * uspUpdateMetadata'
GO

CREATE PROCEDURE [dbo].[uspUpdateMetadata]
	@id int
	, @type nvarchar(20)
	, @attribute nvarchar(20)
	, @label nvarchar(30)
	, @description nvarchar(200)
	, @required bit
	, @protected bit
	AS
		UPDATE
			[HaleDB].[Shared].[Metadata]
		 SET 
			[Type] = @type
			,[Attribute] = @attribute
			,[Label] = @label
			,[Description] = @description
			,[Required] = @required
			,[Protected] = @protected
		WHERE
			[Id] = @id

-- [HaleDB].[uspDeleteMetadata]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - HaleDB-DDL-UserStoredProcedures:   * uspDeleteMetadata'
GO

CREATE PROCEDURE [dbo].[uspDeleteMetadata]
	@id int
	AS
		DELETE FROM [HaleDB].[Shared].[Metadata]
		WHERE Id = @id


