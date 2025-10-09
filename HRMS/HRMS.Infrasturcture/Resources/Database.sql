--#region SP : CheckIfTableExists

IF OBJECT_ID('dbo.CheckIfTableExists', 'P') IS NOT NULL
    DROP PROCEDURE dbo.CheckIfTableExists;
GO
CREATE PROCEDURE dbo.CheckIfTableExists
    @TableName NVARCHAR(128),
    @SchemaName NVARCHAR(128) = 'dbo',
    @IsExists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF (@TableName IS NULL OR LTRIM(RTRIM(@TableName)) = '')
    BEGIN
        RAISERROR('Table name is required.', 16, 1);
        RETURN;
    END;

    IF EXISTS (
        SELECT 1
        FROM sys.tables
        WHERE name = @TableName
          AND schema_id = SCHEMA_ID(@SchemaName)
    )
	BEGIN
		PRINT 'TABLE: ''' + @TableName + ''' already exists.'
        SET @IsExists = 1;
	END
    ELSE
        SET @IsExists = 0;
END;
GO

--#endregion SP : CheckIfTableExists

--#region Tables

DECLARE @Exists BIT;
DECLARE @Table NVARCHAR(128) = 'Roles';

EXEC dbo.CheckIfTableExists
@TableName = @Table,
@IsExists = @Exists OUTPUT

IF(@Exists = 0)
BEGIN 

	create table Roles
	(
		Id      int primary key identity(1,1),
		GuidId  uniqueidentifier default newid() not null,
		Name    varchar(50) not null,
		Description   varchar(500) not null,
		IsActive    bit default 1,

		CreatedDate datetime  not null  default getdate(),
		CreatedUser varchar(50) not null  default '',
		UpdatedDate datetime     default getdate(),
		UpdatedUser varchar(50)   default '',
		IsDeleted bit default 0
	);

END

GO

DECLARE @Exists BIT;
DECLARE @Table NVARCHAR(128) = 'Permissions';

EXEC dbo.CheckIfTableExists
@TableName = @Table,
@IsExists = @Exists OUTPUT

IF(@Exists = 0)
BEGIN 

	create table Permissions
	(
		Id      int primary key identity(1,1),
		GuidId  uniqueidentifier default newid() not null,
		Name    varchar(50) not null,
		Description   varchar(500) not null,
		IsActive    bit default 1,

		CreatedDate datetime  not null  default getdate(),
		CreatedUser varchar(50) not null  default '',
		UpdatedDate datetime     default getdate(),
		UpdatedUser varchar(50)   default '',
		IsDeleted bit default 0
	);

END

GO

create table Users
(
	Id      int primary key identity(1,1),
	GuidId  uniqueidentifier default newid() not null,
	UserName    varchar(50) not null,
	Email       varchar(100) not null,
	Password    varbinary(max),
	HashSalt    varbinary(max),
	Otp         varchar(10),
	OtpDateTime datetime,
	FirstTime   bit default 1,
	EmployeeId  int,
	IsActive    bit default 1,

	CreatedDate datetime  not null  default getdate(),
	CreatedUser varchar(50) not null  default '',
	UpdatedDate datetime     default getdate(),
	UpdatedUser varchar(50)   default '',
	IsDeleted bit default 0,
);

create table RolePermissions
(
	Id      int primary key identity(1,1),
	GuidId  uniqueidentifier default newid() not null,
	RoleId    int not null,
	PermissionId   int not null,

	CreatedDate datetime  not null  default getdate(),
	CreatedUser varchar(50) not null  default '',
	UpdatedDate datetime     default getdate(),
	UpdatedUser varchar(50)   default '',
	IsDeleted bit default 0
);

alter table RolePermissions add constraint FK_RolePermissions_RoleId foreign key (RoleId) references Roles(Id);
alter table RolePermissions add constraint FK_RolePermissions_PermissionId foreign key (PermissionId) references Permissions(Id);

create table UserRoles
(
	Id      int primary key identity(1,1),
	GuidId  uniqueidentifier default newid() not null,
	UserId    int not null,
	RoleId   int not null,

	CreatedDate datetime  not null  default getdate(),
	CreatedUser varchar(50) not null  default '',
	UpdatedDate datetime     default getdate(),
	UpdatedUser varchar(50)   default '',
	IsDeleted bit default 0
);

alter table UserRoles add constraint FK_UserRoles_UserId foreign key (UserId) references Users(Id);
alter table UserRoles add constraint FK_UserRoles_RoleId foreign key (RoleId) references Roles(Id);

create table StoredFiles
(
	Id      int primary key identity(1,1),
	GuidId  uniqueidentifier default newid() not null,
	FileName    varchar(100) not null,
	FileContentType   varchar(50) not null,
	FileExtension varchar(10) not null,
	FileLocationId int,
	IsProcessed    bit default 0,

	CreatedDate datetime  not null  default getdate(),
	CreatedUser varchar(50) not null  default '',
	UpdatedDate datetime     default getdate(),
	UpdatedUser varchar(50)   default '',
	IsDeleted bit default 0
);

create table FileLocationConfigurations
(
	Id      int primary key identity(1,1),
	GuidId  uniqueidentifier default newid() not null,
	ConfigName varchar(100) not null,
	ConfigJson varchar(max) not null,
	IsActive    bit default 1,

	CreatedDate datetime  not null  default getdate(),
	CreatedUser varchar(50) not null  default '',
	UpdatedDate datetime     default getdate(),
	UpdatedUser varchar(50)   default '',
	IsDeleted bit default 0
);

create table SystemSettings
(
	Id      int primary key identity(1,1),
	GuidId  uniqueidentifier default newid() not null,
	SettingKey varchar(100) not null,
	SettingValue varchar(max) not null,
	Description varchar(500) default '',
	IsActive    bit default 1,

	CreatedDate datetime  not null  default getdate(),
	CreatedUser varchar(50) not null  default '',
	UpdatedDate datetime     default getdate(),
	UpdatedUser varchar(50)   default '',
	IsDeleted bit default 0
);

--#endregion

IF NOT EXISTS (
    SELECT 1
    FROM FileLocationConfigurations
    WHERE ConfigName = 'S3BucketConfig'
      AND IsDeleted = 0
)
BEGIN
    INSERT INTO FileLocationConfigurations 
        (GuidId, ConfigName, ConfigJson, IsActive, CreatedUser, IsDeleted)
    VALUES
        (NEWID(), 'S3BucketConfig', '{\n  bucket: \"HRMSAPP\",\n  key: \"HRMSAPPKEY\",\n  secret: \"K005v5PTs8iqSgO1kne3ngkpVBPwCS4\",\n  serviceurl: \"s3.us-east-005.backblazeb2.com\"\n}', 1, 'System', 0);
END