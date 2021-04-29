﻿/* PA_NOTIFICATION ****************************/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_NOTIFICATION' COLLATE SQL_Latin1_General_CP1_CI_AS) )
CREATE TABLE [PA_NOTIFICATION] ( 
	 [Id] INT NOT NULL IDENTITY(1, 1),
	 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [IsRead] BIT NOT NULL DEFAULT(0) ,
	 [UserId] INT NOT NULL DEFAULT(0) ,
	 [NotificationType] INT NOT NULL DEFAULT(0) ,
	 [EntityKey] NVARCHAR(50) ,
	 [Title] NVARCHAR(50) ,
	 [Description] NVARCHAR(50) ,
	 [Comment] NVARCHAR(50) ,
	 [Url] NVARCHAR(50) ,
	 [Target] NVARCHAR(50) ,
CONSTRAINT [PK_PA_NOTIFICATION] PRIMARY KEY CLUSTERED ([Id] ASC)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
;


/* PA_USER ****************************/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_USER' COLLATE SQL_Latin1_General_CP1_CI_AS) )
CREATE TABLE [PA_USER] ( 
	 [FullName] NVARCHAR(50) ,
	 [Role] NVARCHAR(50) ,
	 [Email] NVARCHAR(50) NOT NULL,
	 [PhoneNumber] NVARCHAR(50) ,
	 [EmailConfirmed] BIT NOT NULL DEFAULT(0) ,
	 [EmailConfirmationCode] NVARCHAR(50) ,
	 [PasswordConfirmationCode] NVARCHAR(50) ,
	 [IsInitialPassword] BIT NOT NULL DEFAULT(0) )
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
;


/* PA_Appointment ****************************/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_Appointment' COLLATE SQL_Latin1_General_CP1_CI_AS) )
CREATE TABLE [PA_Appointment] ( 
	 [Id] INT NOT NULL IDENTITY(1, 1),
	 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [Title] NVARCHAR(500) NOT NULL,
	 [CustomerId] INT NOT NULL DEFAULT(0) NOT NULL,
	 [ServiceId] INT NOT NULL DEFAULT(0) NOT NULL,
	 [AllDay] BIT NOT NULL DEFAULT(0) ,
	 [StartDate] DATETIME NOT NULL DEFAULT(GETDATE()) NOT NULL,
	 [EndDate] DATETIME NULL,
	 [State] INT NOT NULL DEFAULT(0),
	 [Priority] INT NOT NULL DEFAULT(0),
	 [Note] NVARCHAR(MAX) ,
	 [CreatedBy] INT NOT NULL DEFAULT(0) ,
	 [UpdatedBy] INT NOT NULL DEFAULT(0) ,
CONSTRAINT [PK_PA_Appointment] PRIMARY KEY CLUSTERED ([Id] ASC)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
;


/* PA_AppointmentEmployee ****************************/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_AppointmentEmployee' COLLATE SQL_Latin1_General_CP1_CI_AS) )
CREATE TABLE [PA_AppointmentEmployee] ( 
	 [Id] INT NOT NULL IDENTITY(1, 1),
	 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [AppointmentId] INT NOT NULL DEFAULT(0) NOT NULL,
	 [UserId] INT NOT NULL DEFAULT(0) NOT NULL,
CONSTRAINT [PK_PA_AppointmentEmployee] PRIMARY KEY CLUSTERED ([Id] ASC)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
;


/* PA_CustomerSummary ****************************/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_CustomerSummary' COLLATE SQL_Latin1_General_CP1_CI_AS) )
CREATE TABLE [PA_CustomerSummary] ( 
	 [CustomerId] INT NOT NULL IDENTITY(1, 1),
	 [Name] NVARCHAR(50) ,
	 [PhoneNumber] NVARCHAR(50) ,
	 [Email] NVARCHAR(50) ,
	 [AppointmentCount] INT NOT NULL DEFAULT(0) ,
	 [CityId] INT NOT NULL DEFAULT(0) ,
	 [LastAppointmentDate] DATETIME NOT NULL DEFAULT(GETDATE()) ,
CONSTRAINT [PK_PA_CustomerSummary] PRIMARY KEY CLUSTERED ([CustomerId] ASC)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
;


/* PA_Document ****************************/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_Document' COLLATE SQL_Latin1_General_CP1_CI_AS) )
CREATE TABLE [PA_Document] ( 
	 [Id] INT NOT NULL IDENTITY(1, 1),
	 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [AppointmentId] INT NOT NULL DEFAULT(0) ,
	 [Name] NVARCHAR(500) NOT NULL,
	 [Size] FLOAT NOT NULL DEFAULT(0) NOT NULL,
	 [Width] INT NULL,
	 [Height] INT NULL,
	 [LastModifiedDate] DATETIME NULL,
	 [FileType] NVARCHAR(16) NOT NULL,
	 [Url] NVARCHAR(1000) NOT NULL,
	 [ThumbnailUrl] NVARCHAR(1000) NOT NULL,
CONSTRAINT [PK_PA_Document] PRIMARY KEY CLUSTERED ([Id] ASC)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
;


/* PA_Service ****************************/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_Service' COLLATE SQL_Latin1_General_CP1_CI_AS) )
CREATE TABLE [PA_Service] ( 
	 [Id] INT NOT NULL IDENTITY(1, 1),
	 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [Name] NVARCHAR(500) NOT NULL,
	 [Color] NVARCHAR(50) ,
	 [Description] NVARCHAR(MAX) ,
CONSTRAINT [PK_PA_Service] PRIMARY KEY CLUSTERED ([Id] ASC)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
;


/* PA_Customer ****************************/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_Customer' COLLATE SQL_Latin1_General_CP1_CI_AS) )
CREATE TABLE [PA_Customer] ( 
	 [Id] INT NOT NULL IDENTITY(1, 1),
	 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [Name] NVARCHAR(250) NOT NULL,
	 [PhoneNumber] NVARCHAR(16) NOT NULL,
	 [Corporate] BIT NOT NULL DEFAULT(0) ,
	 [Title] NVARCHAR(500) ,
	 [Email] NVARCHAR(50) NOT NULL,
	 [LandlinePhoneNumber] NVARCHAR(16) ,
	 [MobilePhoneNumber] NVARCHAR(16) ,
	 [Address] NVARCHAR(500) ,
	 [CityId] INT NOT NULL DEFAULT(0) NOT NULL,
	 [TaxNumber] NVARCHAR(11) ,
	 [TaxAdministration] NVARCHAR(50) ,
	 [BillingAddress] NVARCHAR(500) ,
	 [Note] NVARCHAR(MAX) ,
CONSTRAINT [PK_PA_Customer] PRIMARY KEY CLUSTERED ([Id] ASC)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
;


/* PA_Country ****************************/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_Country' COLLATE SQL_Latin1_General_CP1_CI_AS) )
CREATE TABLE [PA_Country] ( 
	 [Id] INT NOT NULL IDENTITY(1, 1),
	 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [IsActive] BIT NOT NULL DEFAULT(0) ,
	 [Alpha2] NVARCHAR(2) NOT NULL,
	 [Alpha3] NVARCHAR(3) ,
	 [UNCode] NVARCHAR(16) ,
	 [CallingCode] NVARCHAR(8) ,
	 [Name] NVARCHAR(50) NOT NULL,
	 [Image] NVARCHAR(1000) ,
CONSTRAINT [PK_PA_Country] PRIMARY KEY CLUSTERED ([Id] ASC)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
;


/* PA_City ****************************/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_City' COLLATE SQL_Latin1_General_CP1_CI_AS) )
CREATE TABLE [PA_City] ( 
	 [Id] INT NOT NULL IDENTITY(1, 1),
	 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
	 [CountryId] INT NOT NULL DEFAULT(0) NOT NULL,
	 [Code] NVARCHAR(3) NOT NULL,
	 [Name] NVARCHAR(50) NOT NULL,
CONSTRAINT [PK_PA_City] PRIMARY KEY CLUSTERED ([Id] ASC)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
SET ANSI_NULLS OFF
SET QUOTED_IDENTIFIER OFF
;

