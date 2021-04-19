/*
IF (1 = 0)
BEGIN
	-- ADD COLUMN SAMPLE ***********************
	IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
		WHERE t.name = 'DNA_CENTER' AND c.name = 'Id')
	BEGIN
		ALTER TABLE [dbo].[DNA_TITAN_CENTER] ADD Id INT IDENTITY(1,1) NOT NULL;
	END

	-- INSERT TOW TABLE ***********************
	IF NOT EXISTS(SELECT * FROM [dbo].DNA_CENTER WHERE [CenterNumber] = 9382)
	BEGIN
		INSERT [dbo].DNA_CENTER ([CenterNumber], [CenterName], [LegalEntity], [SunBussinesUnitCode], [Sequence], [SupplierId]) VALUES (9382, N'OPS Centre Turkey - TRY', N'Regus Yonetim ve Danismanlik Ltd Sti', N'V89', 4, 3)
	END
END
*/

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{TablePrefix}Appointment]') AND type in (N'U'))
BEGIN
	/* PA_Appointment ****************************/
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON
	IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_Appointment' COLLATE SQL_Latin1_General_CP1_CI_AS) )
	CREATE TABLE [PA_Appointment] ( 
		 [Id] INT NOT NULL IDENTITY(1, 1),
		 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [CustomerId] INT NOT NULL DEFAULT(0),
		 [ServiceId] INT NOT NULL DEFAULT(0),
		 [StartDate] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [EndDate] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [State] INT NOT NULL DEFAULT(0),
		 [Priority] INT NOT NULL DEFAULT(0),
		 [Note] NVARCHAR(MAX),
		 [CreatedBy] INT NOT NULL DEFAULT(0) ,
	CONSTRAINT [PK_PA_Appointment] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	SET ANSI_NULLS OFF
	SET QUOTED_IDENTIFIER OFF
END

/* PA_Appointment UpdatedBy ****************************/
IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
	WHERE t.name = '{TablePrefix}Appointment' AND c.name = 'UpdatedBy')
BEGIN
	ALTER TABLE [dbo].[{TablePrefix}Appointment] ADD UpdatedBy INT NOT NULL DEFAULT(0);
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{TablePrefix}AppointmentEmployee]') AND type in (N'U'))
BEGIN
	/* PA_AppointmentEmployee ****************************/
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON
	IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_AppointmentEmployee' COLLATE SQL_Latin1_General_CP1_CI_AS) )
	CREATE TABLE [PA_AppointmentEmployee] ( 
		 [Id] INT NOT NULL IDENTITY(1, 1),
		 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [AppointmentId] INT NOT NULL DEFAULT(0),
		 [UserId] INT NOT NULL DEFAULT(0),
	CONSTRAINT [PK_PA_AppointmentEmployee] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	SET ANSI_NULLS OFF
	SET QUOTED_IDENTIFIER OFF
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{TablePrefix}Document]') AND type in (N'U'))
BEGIN
	/* PA_Document ****************************/
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON
	IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_Document' COLLATE SQL_Latin1_General_CP1_CI_AS) )
	CREATE TABLE [PA_Document] ( 
		 [Id] INT NOT NULL IDENTITY(1, 1),
		 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [AppointmentId] INT NOT NULL DEFAULT(0) ,
		 [FileType] NVARCHAR(16) NOT NULL,
		 [Url] NVARCHAR(1000) NOT NULL,
	CONSTRAINT [PK_PA_Document] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	SET ANSI_NULLS OFF
	SET QUOTED_IDENTIFIER OFF
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{TablePrefix}Service]') AND type in (N'U'))
BEGIN
	/* PA_Service ****************************/
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON
	IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_Service' COLLATE SQL_Latin1_General_CP1_CI_AS) )
	CREATE TABLE [PA_Service] ( 
		 [Id] INT NOT NULL IDENTITY(1, 1),
		 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [Name] NVARCHAR(500) NOT NULL,
		 [Description] NVARCHAR(MAX) ,
	CONSTRAINT [PK_PA_Service] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	SET ANSI_NULLS OFF
	SET QUOTED_IDENTIFIER OFF

	INSERT INTO [{TablePrefix}Service] ([Name]) VALUES
		(N'Ev Temizliği'),
		(N'Dış Cephe Cam Temizliği'),
		(N'Villa Temizliği'),
		(N'Otel, Pansiyon Temizliği'),
		(N'Apartman Temizliği'),
		(N'Ev Ve İşyeri Dezenfeksiyon')
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{TablePrefix}Customer]') AND type in (N'U'))
BEGIN
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
		 [Corporate] BIT DEFAULT(0) ,
		 [Title] NVARCHAR(500) ,
		 [Email] NVARCHAR(50) NOT NULL,
		 [LandlinePhoneNumber] NVARCHAR(16) ,
		 [MobilePhoneNumber] NVARCHAR(16) ,
		 [Address] NVARCHAR(500) ,
		 [CityId] INT NOT NULL DEFAULT(0) ,
		 [TaxNumber] NVARCHAR(11) ,
		 [TaxAdministration] NVARCHAR(50) ,
		 [BillingAddress] NVARCHAR(500) ,
		 [Note] NVARCHAR(MAX) ,
	CONSTRAINT [PK_PA_Customer] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	SET ANSI_NULLS OFF
	SET QUOTED_IDENTIFIER OFF
END


IF NOT EXISTS (
	SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{TablePrefix}Country]') AND type in (N'U')
)
BEGIN

	/* PA_Country ****************************/
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON
	IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_Country' COLLATE SQL_Latin1_General_CP1_CI_AS) )
	CREATE TABLE [PA_Country] ( 
		 [Id] INT NOT NULL IDENTITY(1, 1),
		 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [IsActive] BIT DEFAULT(0) ,
		 [Alpha2] NVARCHAR(2) NOT NULL,
		 [Alpha3] NVARCHAR(3) ,
		 [UNCode] NVARCHAR(16) ,
		 [CallingCode] NVARCHAR(8) ,
		 [Name] NVARCHAR(50) NOT NULL
	CONSTRAINT [PK_PA_Country] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	SET ANSI_NULLS OFF
	SET QUOTED_IDENTIFIER OFF

	INSERT INTO [{TablePrefix}Country] ([IsActive], [Alpha2], [Alpha3], [UNCode], [CallingCode], [Name]) VALUES 
		(CAST(1 AS BIT), 'TR', 'TUR', '792', '90', N'Türkiye')
END
		
IF NOT EXISTS (
	SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{TablePrefix}City]') AND type in (N'U')
)
BEGIN

	/* PA_City ****************************/
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON
	IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('PA_City' COLLATE SQL_Latin1_General_CP1_CI_AS) )
	CREATE TABLE [PA_City] ( 
		 [Id] INT NOT NULL IDENTITY(1, 1),
		 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()) ,
		 [CountryId] INT NOT NULL DEFAULT(0),
		 [Code] NVARCHAR(3) NOT NULL,
		 [Name] NVARCHAR(50) NOT NULL,
	CONSTRAINT [PK_PA_City] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	SET ANSI_NULLS OFF
	SET QUOTED_IDENTIFIER OFF

	INSERT INTO [{TablePrefix}City] ([CountryId], [Code], [Name]) VALUES
		(1, '01', N'Adana'),
		(1, '02', N'Adıyaman'),
		(1, '03', N'Afyon'),
		(1, '04', N'Ağrı'),
		(1, '05', N'Amasya'),
		(1, '06', N'Ankara'),
		(1, '07', N'Antalya'),
		(1, '08', N'Artvin'),
		(1, '09', N'Aydın'),
		(1, '10', N'Balıkesir'),
		(1, '11', N'Bilecik'),
		(1, '12', N'Bingöl'),
		(1, '13', N'Bitlis'),
		(1, '14', N'Bolu'),
		(1, '15', N'Burdur'),
		(1, '16', N'Bursa'),
		(1, '17', N'Çanakkale'),
		(1, '18', N'Çankırı'),
		(1, '19', N'Çorum'),
		(1, '20', N'Denizli'),
		(1, '21', N'Diyarbakır'),
		(1, '22', N'Edirne'),
		(1, '23', N'Elazığ'),
		(1, '24', N'Erzincan'),
		(1, '25', N'Erzurum'),
		(1, '26', N'Eskişehir'),
		(1, '27', N'Gaziantep'),
		(1, '28', N'Giresun'),
		(1, '29', N'Gümüşhane'),
		(1, '30', N'Hakkari'),
		(1, '31', N'Hatay'),
		(1, '32', N'Isparta'),
		(1, '33', N'Mersin'),
		(1, '34', N'İstanbul'),
		(1, '35', N'İzmir'),
		(1, '36', N'Kars'),
		(1, '37', N'Kastamonu'),
		(1, '38', N'Kayseri'),
		(1, '39', N'Kırklareli'),
		(1, '40', N'Kırşehir'),
		(1, '41', N'Kocaeli'),
		(1, '42', N'Konya'),
		(1, '43', N'Kütahya'),
		(1, '44', N'Malatya'),
		(1, '45', N'Manisa'),
		(1, '46', N'K.Maraş'),
		(1, '47', N'Mardin'),
		(1, '48', N'Muğla'),
		(1, '49', N'Muş'),
		(1, '50', N'Nevşehir'),
		(1, '51', N'Niğde'),
		(1, '52', N'Ordu'),
		(1, '53', N'Rize'),
		(1, '54', N'Sakarya'),
		(1, '55', N'Samsun'),
		(1, '56', N'Siirt'),
		(1, '57', N'Sinop'),
		(1, '58', N'Sivas'),
		(1, '59', N'Tekirdağ'),
		(1, '60', N'Tokat'),
		(1, '61', N'Trabzon'),
		(1, '62', N'Tunceli'),
		(1, '63', N'Şanlıurfa'),
		(1, '64', N'Uşak'),
		(1, '65', N'Van'),
		(1, '66', N'Yozgat'),
		(1, '67', N'Zonguldak'),
		(1, '68', N'Aksaray'),
		(1, '69', N'Bayburt'),
		(1, '70', N'Karaman'),
		(1, '71', N'Kırıkkale'),
		(1, '72', N'Batman'),
		(1, '73', N'Şırnak'),
		(1, '74', N'Bartın'),
		(1, '75', N'Ardahan'),
		(1, '76', N'Iğdır'),
		(1, '77', N'Yalova'),
		(1, '78', N'Karabük'),
		(1, '79', N'Kilis'),
		(1, '80', N'Osmaniye'),
		(1, '81', N'Düzce')
END