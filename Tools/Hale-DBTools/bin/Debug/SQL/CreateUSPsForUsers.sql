USE HaleDB


-- HaleDB.uspGetUserById @id
GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspGetUserById'
GO

CREATE PROCEDURE [uspGetUserById]
	@Id int
	AS
		SELECT
			Id
			, Username
			, Email
			, Password
			, Salt
			, Encryption
			, Created
			, CreatedBy
			, Changed
			, ChangedBy
		FROM
			[HaleDB].[Security].[Users]
		WHERE
			Id = @Id

-- HaleDB.uspGetUserByName @id
GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspGetUserByName'
GO

CREATE PROCEDURE [uspGetUserByName]
	@Name nvarchar(25)
	AS
		SELECT
			Id
			, Username
			, Email
			, Password
			, Salt
			, Encryption
			, Created
			, CreatedBy
			, Changed
			, ChangedBy
		FROM
			[HaleDB].[Security].[Users]
		WHERE
			Username = @Name
	

--- [HaleDB].[uspGetUsers]
GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspListUsers'
GO

CREATE PROCEDURE [uspListUsers]
	AS
		SELECT
			Id
			, Username
			, Email
			, Password
			, Salt
			, Encryption
			, Created
			, CreatedBy
			, Changed
			, ChangedBy
		FROM
			[HaleDB].[Security].[Users]

--- [HaleDB].[uspAddUser]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspAddUser'
GO

CREATE PROCEDURE [uspAddUser]
	@Username nvarchar(25)
	, @Email nvarchar(100)
	, @Password nvarchar(88)
	, @Salt nvarchar(88)
	, @Encryption nvarchar(10)
	, @CreatedBy int
	AS
		INSERT INTO 
			[HaleDB].[Security].[Users]
			(
				Username
				, Email
				, Password
				, Salt
				, Encryption
				, Created
				, CreatedBy
				, Changed
				, ChangedBy
			)
			VALUES
			(
				@Username
				, @Email 
				, @Password 
				, @Salt 
				, @Encryption
				, SYSDATETIME()
				, @CreatedBy 
				, SYSDATETIME()
				, @CreatedBy 
			)

-- [HaleDB].[uspDeleteUser]


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspDeleteUser'
GO

CREATE PROCEDURE [uspDeleteUser]
	@Id int
	AS
		DELETE FROM
			[HaleDB].[Security].[UserDetails]
		WHERE
			[UserId] = @Id

		DELETE FROM
			[HaleDB].[Security].[Users]
		WHERE
			[Id] = @Id

-- [HaleDB].[uspUpdateUser]


GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspUpdateUser'
GO

CREATE PROCEDURE [uspUpdateUser]
	@Id int
	, @username nvarchar(25)
	, @email nvarchar(100)
	, @password nvarchar(88)
	, @salt nvarchar(88)
	AS
		UPDATE [HaleDB].[Security].[Users]
			SET
				[Username] = @username
				, [Email] = @email
				, [Password] = @password
				, [Salt] = @salt
		WHERE
			Id = @Id



-- [HaleDB].[uspCreateUserDetail]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspCreateUserDetail'
GO

CREATE PROCEDURE [uspCreateUserDetail]
	@UserId int
	, @Key nvarchar(100)
	, @Value nvarchar(max)
	AS
		INSERT INTO
			[HaleDB].[Security].[UserDetails]
			(
				[UserId]
				, [Key]
				, [Value]
			)
			VALUES
			(
				@UserId
				, @Key
				, @Value
			)

-- [HaleDB].[uspUpdateUserDetail]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspUpdateUserDetail'
GO

CREATE PROCEDURE [uspUpdateUserDetail]
	@UserId int
	, @Key nvarchar(100)
	, @Value nvarchar(max)
	AS
		UPDATE [HaleDB].[Security].[UserDetails]
			SET [Value] = @Value
		WHERE
			[UserId] = @UserId
			AND
			[Key] = @Key
			


-- [HaleDB].[uspGetUserDetail]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspGetUserDetail'
GO

CREATE PROCEDURE [uspGetUserDetail]
	@UserId int
	, @Key nvarchar(100)
	AS
		SELECT
			[Id]
			, [UserId]
			, [Key]
			, [Value]
		FROM
			[HaleDB].[Security].[UserDetails]
		WHERE
			[UserId] = @UserId
			AND
			[Key] = @Key


-- [HaleDB].[uspListUserDetails]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspListUserDetails'
GO

CREATE PROCEDURE [uspListUserDetails]
	@UserId int
	AS
		SELECT
			[Id]
			, [UserId]
			, [Key]
			, [Value]
		FROM
			[HaleDB].[Security].[UserDetails]
		WHERE
			[UserId] = @UserId

-- [HaleDB].[uspDeleteUserDetail]

GO
PRINT CAST(SYSDATETIME() AS NVARCHAR(20)) + ' - Created uspDeleteUserDetail'
GO

CREATE PROCEDURE [uspDeleteUserDetail]
	@UserId int
	, @Key nvarchar(100)
	AS
		DELETE FROM
			[HaleDB].[Security].[UserDetails]
		WHERE
			[UserId] = @UserId
			AND
			[Key] = @Key
