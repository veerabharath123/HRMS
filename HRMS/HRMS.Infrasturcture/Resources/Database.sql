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