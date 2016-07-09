USE HaleDB

-- [HaleDB].[uspCreateHost]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspCreateHost'
GO

CREATE PROCEDURE [uspCreateHost]
	@Name nvarchar(50)
	, @HostName nvarchar(150)
	, @IP nvarchar(15)
	AS
		INSERT INTO
			HaleDB.Nodes.Hosts
			(
				Name
				, HostName
				, IP
				, [Status]
				, Updated
				, Added
			)
			VALUES
			(
				@Name
				, @HostName
				, @IP
				, -1
				, SYSDATETIME()
				, SYSDATETIME()
			)



-- [HaleDB].[uspGetHost]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspGetHost'
GO

CREATE PROCEDURE [uspGetHost]
	@Id int
	
	AS
		SELECT TOP 1
			Id
			, Name
			, HostName
			, Ip
			, LastSeen
			, [Status]
			, [Updated]
			, [Added]
			, [Guid]
			, [RsaKey]
		FROM
			[HaleDB].[Nodes].[Hosts]
		WHERE
			Id = @Id


-- [HaleDB].[uspGetHostByGuid]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspGetHostByGuid'
GO

CREATE PROCEDURE [uspGetHostByGuid]
	@Guid uniqueidentifier
	
	AS
		SELECT TOP 1
			Id
			, Name
			, HostName
			, Ip
			, LastSeen
			, [Status]
			, [Updated]
			, [Added]
			, [Guid]
			, [RsaKey]
		FROM
			[HaleDB].[Nodes].[Hosts]
		WHERE
			Guid = @Guid


-- [HaleDB].[uspListHosts]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspListHosts'
GO

CREATE PROCEDURE [uspListHosts]
	@count int = -1
	AS
		IF @count = -1
			BEGIN
				SELECT
					Id
					, Name
					, HostName
					, Ip
					, LastSeen
					, [Status]
					, [Updated]
					, [Added]
					, [Guid]
					, [RsaKey]
				FROM
					[HaleDB].[Nodes].[Hosts]
			END
		ELSE
			BEGIN
				SELECT TOP (@count)
					Id
					, Name
					, HostName
					, Ip
					, LastSeen
					, [Status]
					, [Updated]
					, [Added]
					, [Guid]
					, [RsaKey]
				FROM
					[HaleDB].[Nodes].[Hosts]
			END

-- [HaleDB].[uspDeleteHost]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspDeleteHost'
GO

CREATE PROCEDURE [uspDeleteHost]
	@Id int
	AS
		DELETE FROM
			[HaleDB].[Nodes].Hosts
		WHERE
			Id = @Id


-- [HaleDB].[uspGetHostDetail]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspGetHostDetail'
GO

CREATE PROCEDURE [dbo].[uspGetHostDetail]
	@HostId int
	AS
		SELECT
			[Id]
			, [HostId]
			, [Key]
			, [Value]
		FROM
			[HaleDB].[Nodes].[HostDetails]
		WHERE
			[HostId] = @HostId

			
-- [HaleDB].[uspCreateHostDetails]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspCreateHostDetails'
GO

CREATE PROCEDURE [dbo].[uspCreateHostDetail]
	@HostId int
	, @Key nvarchar(100)
	, @Value nvarchar(max)
	AS
		INSERT INTO
			[HaleDB].[Nodes].[HostDetails]
			(
				[HostId]
				, [Key]
				, [Value]
			)
			VALUES
			(
				@HostId
				, @Key
				, @Value
			)


-- [HaleDB].[uspUpdateHostDetail]


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspUpdateHostDetail'
GO

CREATE PROCEDURE [uspUpdateHostDetail]
	@Id int
	, @Key nvarchar(100)
	, @Value nvarchar(200)
	AS
		BEGIN
			UPDATE [HaleDB].[Nodes].[HostDetails]
				SET [Value] = @value
			WHERE
				[Id] = @Id
				AND
				[Key] = @Key
		END



-- [HaleDB].[uspListHostDetails]


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspListHostDetails'
GO

CREATE PROCEDURE [uspListHostDetails]
	@hostId int
	AS
		BEGIN
			SELECT
				[Id]
				, [HostId]
				, [Key]
				, [Value]
			FROM
				[HaleDB].[Nodes].[HostDetails]
			WHERE
				[HostId] = @hostId
		END




-- [HaleDB].[uspUpdateHost]


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspUpdateHost'
GO

CREATE PROCEDURE [uspUpdateHost]
	@Id int
	, @Name [nvarchar](50)
	, @HostName [nvarchar](150)
	, @Ip [nvarchar](15)
	, @Guid [uniqueidentifier]
	
	AS
		BEGIN
			UPDATE [HaleDB].[Nodes].[Hosts]
				SET
					[Name] = @Name
					, [HostName] = @HostName
					, [Ip] = @Ip
					, [Updated] = SYSDATETIME()
					, [Guid] = @Guid
			WHERE
				Id = @Id
		END

-- [HaleDB].[uspUpdateHostStatus]


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspUpdateHostStatus'
GO

CREATE PROCEDURE [uspUpdateHostStatus]
	@Id int
	, @status int
	AS
		IF @status <> 0
		BEGIN
			UPDATE [HaleDB].[Nodes].[Hosts]
				SET [Status] = @status
			WHERE
				Id = @Id
		END
		ELSE
		BEGIN
			UPDATE [HaleDB].[Nodes].[Hosts]
				SET [Status] = @status
				, [LastSeen] = SYSDATETIME() 
			WHERE
				Id = @Id
		END
		


-- [HaleDB].[uspUpdateHostRsaKey]


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspUpdateHostRsaKey'
GO

CREATE PROCEDURE [uspUpdateHostRsaKey]
	@Id int
	, @rsaKey varbinary(1024)
	AS
		UPDATE [HaleDB].[Nodes].[Hosts]
			SET [RsaKey] = @rsaKey
		WHERE
			Id = @Id
