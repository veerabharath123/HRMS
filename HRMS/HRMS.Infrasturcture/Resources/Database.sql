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

GO

DECLARE @Exists BIT;
DECLARE @Table NVARCHAR(128) = 'GeneralReference';

EXEC dbo.CheckIfTableExists
@TableName = @Table,
@IsExists = @Exists OUTPUT

IF(@Exists = 0)
BEGIN 

	CREATE TABLE GeneralReference (
		Id INT IDENTITY(1,1) PRIMARY KEY,         -- internal key for joins
		GuidId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), -- external reference

		Category VARCHAR(50) NOT NULL,            -- e.g., 'Gender', 'MaritalStatus'
		Code VARCHAR(20) NOT NULL DEFAULT '',                -- e.g., 'M', 'SINGLE'
		[Value] VARCHAR(20) NOT NULL,
		Description VARCHAR(100) DEFAULT '',        -- e.g., 'Male', 'Single'
		IsActive BIT NOT NULL DEFAULT 1,
		SortOrder INT DEFAULT 0,
		
		CreatedDate datetime  not null  default getdate(),
		CreatedUser varchar(50) not null  default '',
		UpdatedDate datetime     default getdate(),
		UpdatedUser varchar(50)   default '',
		IsDeleted bit default 0
	);

END

GO

DECLARE @Exists BIT;
DECLARE @Table NVARCHAR(128) = 'Department';

EXEC dbo.CheckIfTableExists
@TableName = @Table,
@IsExists = @Exists OUTPUT

IF(@Exists = 0)
BEGIN 

	CREATE TABLE Department (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		GuidId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),

		Name VARCHAR(100) NOT NULL,
		Code VARCHAR(20),
		Description VARCHAR(200),
		IsActive BIT DEFAULT 1,

		CreatedDate datetime  not null  default getdate(),
		CreatedUser varchar(50) not null  default '',
		UpdatedDate datetime     default getdate(),
		UpdatedUser varchar(50)   default '',
		IsDeleted bit default 0
	);

END

GO

DECLARE @Exists BIT;
DECLARE @Table NVARCHAR(128) = 'Designation';

EXEC dbo.CheckIfTableExists
@TableName = @Table,
@IsExists = @Exists OUTPUT

IF(@Exists = 0)
BEGIN 

	CREATE TABLE Designation (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		GuidId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),

		Name VARCHAR(100) NOT NULL,
		DepartmentId INT NULL FOREIGN KEY REFERENCES Department(Id),
		Description VARCHAR(200),
		IsActive BIT DEFAULT 1,

		CreatedDate datetime  not null  default getdate(),
		CreatedUser varchar(50) not null  default '',
		UpdatedDate datetime     default getdate(),
		UpdatedUser varchar(50)   default '',
		IsDeleted bit default 0
	);


END

GO

DECLARE @Exists BIT;
DECLARE @Table NVARCHAR(128) = 'Employee';

EXEC dbo.CheckIfTableExists
@TableName = @Table,
@IsExists = @Exists OUTPUT

IF(@Exists = 0)
BEGIN 

	create table Employee
	(
		Id      int primary key identity(1,1),
		GuidId  uniqueidentifier default newid() not null,

		--personal details
		LastName    varchar(50) not null,
		MiddleName    varchar(50) not null,
		FirstName    varchar(50) not null,
		BirthDate   datetime not null,
		GenderId INT NULL FOREIGN KEY REFERENCES GeneralReference(Id),
		MaritalStatusId INT NULL FOREIGN KEY REFERENCES GeneralReference(Id),
		PhotoPictureId INT NULL FOREIGN KEY REFERENCES StoredFiles(Id),
		Bio VARCHAR(1000) NULL,

		--organizational details
		DepartmentId    int FOREIGN KEY REFERENCES Department(Id),
		DesignationId   int FOREIGN KEY REFERENCES Designation(Id),
		JoiningDate datetime not null,
		RelievingDate datetime null,
		ResignationDate datetime null,
		ReportingManagerId INT NULL FOREIGN KEY REFERENCES Employee(Id),

		CreatedDate datetime  not null  default getdate(),
		CreatedUser varchar(50) not null  default '',
		UpdatedDate datetime     default getdate(),
		UpdatedUser varchar(50)   default '',
		IsDeleted bit default 0
	);

END


GO

DECLARE @Exists BIT;
DECLARE @Table NVARCHAR(128) = 'EmployeeContact';

EXEC dbo.CheckIfTableExists
@TableName = @Table,
@IsExists = @Exists OUTPUT

IF(@Exists = 0)
BEGIN 

	CREATE TABLE EmployeeContact (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		GuidId  uniqueidentifier default newid() not null,

		EmployeeId INT NOT NULL FOREIGN KEY REFERENCES Employee(Id),
		ContactTypeId INT NOT NULL FOREIGN KEY REFERENCES GeneralReference(Id), -- e.g., Mobile, Email
		ContactValue VARCHAR(100) NOT NULL,
		[Description] VARCHAR(255),
		IsPrimary BIT DEFAULT 0,

		CreatedDate datetime  not null  default getdate(),
		CreatedUser varchar(50) not null  default '',
		UpdatedDate datetime     default getdate(),
		UpdatedUser varchar(50)   default '',
		IsDeleted bit default 0
	);

END


GO

DECLARE @Exists BIT;
DECLARE @Table NVARCHAR(128) = 'EmployeeAddress';

EXEC dbo.CheckIfTableExists
@TableName = @Table,
@IsExists = @Exists OUTPUT

IF(@Exists = 0)
BEGIN 

	CREATE TABLE EmployeeAddress (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		GuidId  uniqueidentifier default newid() not null,

		EmployeeId INT NOT NULL FOREIGN KEY REFERENCES Employee(Id),
		AddressTypeId INT NOT NULL FOREIGN KEY REFERENCES GeneralReference(Id), -- e.g., Permanent, Present
		AddressLine1 VARCHAR(100) NOT NULL,
		AddressLine2 VARCHAR(100),
		City VARCHAR(50),
		State VARCHAR(50),
		Country VARCHAR(50),
		PostalCode VARCHAR(10),
    
		CreatedDate datetime  not null  default getdate(),
		CreatedUser varchar(50) not null  default '',
		UpdatedDate datetime     default getdate(),
		UpdatedUser varchar(50)   default '',
		IsDeleted bit default 0
	);

END

GO

DECLARE @Exists BIT;
DECLARE @Table NVARCHAR(128) = 'EmployeeGuardian';

EXEC dbo.CheckIfTableExists
@TableName = @Table,
@IsExists = @Exists OUTPUT

IF(@Exists = 0)
BEGIN 

	CREATE TABLE EmployeeGuardian
	(
		Id INT IDENTITY(1,1) PRIMARY KEY,
		GuidId  uniqueidentifier default newid() not null,

		EmployeeId INT NOT NULL FOREIGN KEY REFERENCES Employee(Id),
		[Name] VARCHAR(100) NOT NULL,
		RelationshipId INT NOT NULL FOREIGN KEY REFERENCES GeneralReference(Id), -- Father, Mother, Guardian, etc.
		Phone VARCHAR(20) NULL,
		Email VARCHAR(100) NULL,

		CreatedDate datetime  not null  default getdate(),
		CreatedUser varchar(50) not null  default '',
		UpdatedDate datetime     default getdate(),
		UpdatedUser varchar(50)   default '',
		IsDeleted bit default 0
	);

END

--#endregion

--# region Insert/Update SP

IF OBJECT_ID('dbo.InsertSystemSettingIfNotExists', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.InsertSystemSettingIfNotExists;
END
GO

CREATE PROCEDURE dbo.InsertSystemSettingIfNotExists
    @SettingKey VARCHAR(100),
    @SettingValue VARCHAR(MAX),
    @SettingDescription VARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM SystemSettings
        WHERE SettingKey = @SettingKey
          AND IsDeleted = 0
    )
    BEGIN
        INSERT INTO SystemSettings 
            (GuidId, SettingKey, SettingValue, [Description], IsActive, CreatedUser, IsDeleted)
        VALUES
            (
                NEWID(), 
                @SettingKey, 
                @SettingValue, 
                ISNULL(@SettingDescription, ''), 
                1, 
                'System', 
                0
            );
    END
   ELSE PRINT 'Setting: ''' + @SettingKey + ''' already exists.'
END
GO

--#endregion Insert/Update SP


EXEC InsertSystemSettingIfNotExists 
'FileStorageLocation', 
'1', 
'File storage location id';
	

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
        (NEWID(), 'S3BucketConfig', '{"bucket":"HRMSAPP","key":"HRMSAPPKEY","serviceurl":"K005v5PTs8iqSgO1kne3ngkpVBPwCS4","secret":"s3.us-east-005.backblazeb2.com"}', 1, 'System', 0);
END

IF NOT EXISTS (
    SELECT 1
    FROM FileLocationConfigurations
    WHERE ConfigName = 'FtpLocalConfig'
      AND IsDeleted = 0
)
BEGIN
    INSERT INTO FileLocationConfigurations 
        (GuidId, ConfigName, ConfigJson, IsActive, CreatedUser, IsDeleted)
    VALUES
        (NEWID(), 'FtpLocalConfig', '{"FtpBaseUrl":"hrmsftp.local","FtpUsername":"VeeraBharath","FtpPassword":"ftppswd001"}', 1, 'System', 0);
END

IF OBJECT_ID('dbo.GeneralReference', 'U') IS NOT NULL
BEGIN
    TRUNCATE TABLE dbo.GeneralReference;

    -- GENDER
    INSERT INTO dbo.GeneralReference (Category, Code, [Value], Description, SortOrder, CreatedUser)
    VALUES
    ('Gender', 'M', 'Male', 'Male gender', 1, 'System'),
    ('Gender', 'F', 'Female', 'Female gender', 2, 'System'),
    ('Gender', 'O', 'Other', 'Other / Non-binary', 3, 'System');

    -- MARITAL STATUS
    INSERT INTO dbo.GeneralReference (Category, Code, [Value], Description, SortOrder, CreatedUser)
    VALUES
    ('MaritalStatus', 'S', 'Single', 'Unmarried', 1, 'System'),
    ('MaritalStatus', 'M', 'Married', 'Married', 2, 'System'),
    ('MaritalStatus', 'D', 'Divorced', 'Legally divorced', 3, 'System'),
    ('MaritalStatus', 'W', 'Widowed', 'Widowed', 4, 'System');

	-- RELATIONSHIP STATUS
	INSERT INTO dbo.GeneralReference (Category, Code, [Value], Description, IsActive, SortOrder, CreatedUser)
    VALUES
    ('Relationship', 'FATHER', 'Father', 'Biological or adoptive father', 1, 1, 'System'),
    ('Relationship', 'MOTHER', 'Mother', 'Biological or adoptive mother', 1, 2, 'System'),
    ('Relationship', 'GUARDIAN', 'Guardian', 'Legal guardian or caretaker', 1, 3, 'System'),
    ('Relationship', 'SPOUSE', 'Spouse', 'Husband or wife', 1, 4, 'System'),
    ('Relationship', 'CHILD', 'Child', 'Son or daughter', 1, 5, 'System'),
    ('Relationship', 'OTHER', 'Other', 'Any other relationship', 1, 6, 'System');
END

GO

ALTER TABLE dbo.Designation NOCHECK CONSTRAINT ALL;

IF OBJECT_ID('dbo.Department', 'U') IS NOT NULL
BEGIN
    DELETE FROM dbo.Department;

    INSERT INTO dbo.Department (Name, Code, Description, IsActive, CreatedUser)
    VALUES
    ('Human Resources', 'HR', 'Handles recruitment, payroll, and employee relations', 1, 'System'),
    ('Finance', 'FIN', 'Manages company finances, budgets, and accounts', 1, 'System'),
    ('Information Technology', 'IT', 'Develops and maintains software and infrastructure', 1, 'System'),
    ('Quality Assurance', 'QA', 'Ensures product quality through testing', 1, 'System'),
    ('Operations', 'OPS', 'Oversees daily operational activities', 1, 'System'),
    ('Management', 'MGMT', 'Project ownership and leadership', 1, 'System');
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Department] does not exist.';
END;

IF OBJECT_ID('dbo.Designation', 'U') IS NOT NULL
BEGIN
    DELETE FROM dbo.Designation;

    INSERT INTO dbo.Designation (Name, DepartmentId, Description, IsActive, CreatedUser)
    SELECT 'HR Manager', Id, 'Leads HR Department', 1, 'System' FROM dbo.Department WHERE Code = 'HR'
    UNION ALL
    SELECT 'Recruiter', Id, 'Handles candidate recruitment and onboarding', 1, 'System' FROM dbo.Department WHERE Code = 'HR'
    UNION ALL
    SELECT 'Finance Manager', Id, 'Manages accounting and budgets', 1, 'System' FROM dbo.Department WHERE Code = 'FIN'
    UNION ALL
    SELECT 'Accountant', Id, 'Maintains financial records', 1, 'System' FROM dbo.Department WHERE Code = 'FIN'
    UNION ALL
    SELECT 'Software Developer', Id, 'Develops and maintains applications and systems', 1, 'System' FROM dbo.Department WHERE Code = 'IT'
    UNION ALL
    SELECT 'Team Lead', Id, 'Leads the development team', 1, 'System' FROM dbo.Department WHERE Code = 'IT'
    UNION ALL
    SELECT 'System Administrator', Id, 'Maintains IT infrastructure and networks', 1, 'System' FROM dbo.Department WHERE Code = 'IT'
    UNION ALL
    SELECT 'QA Engineer', Id, 'Tests applications for quality and performance', 1, 'System' FROM dbo.Department WHERE Code = 'QA'
    UNION ALL
    SELECT 'QA Lead', Id, 'Leads QA team and testing strategies', 1, 'System' FROM dbo.Department WHERE Code = 'QA'
    UNION ALL
    SELECT 'Operations Executive', Id, 'Monitors and supports daily operations', 1, 'System' FROM dbo.Department WHERE Code = 'OPS'
    UNION ALL
    SELECT 'Operations Manager', Id, 'Leads operations and logistics', 1, 'System' FROM dbo.Department WHERE Code = 'OPS'
    UNION ALL
    SELECT 'Project Manager', Id, 'Manages project scope, timeline, and resources', 1, 'System' FROM dbo.Department WHERE Code = 'MGMT'
    UNION ALL
    SELECT 'Project Owner', Id, 'Owns and oversees project delivery', 1, 'System' FROM dbo.Department WHERE Code = 'MGMT'
    UNION ALL
    SELECT 'Team Lead', Id, 'Leads team members and coordinates with management', 1, 'System' FROM dbo.Department WHERE Code = 'MGMT';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Designation] does not exist.';
END;

ALTER TABLE dbo.Designation WITH CHECK CHECK CONSTRAINT ALL;