IF (1 = 0)
BEGIN

	-- CREATE TABLE SAMPLE ***********************
	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DNA_CENTER]') AND type in (N'U'))
	BEGIN
		CREATE TABLE [dbo].DNA_CENTER (
			[CenterNumber] [int] NOT NULL,
			[CenterName] [nvarchar](200) NOT NULL,
			[LegalEntity] [nvarchar](200) NOT NULL,
			[SunBussinesUnitCode] [nvarchar](50) NOT NULL,
			[Sequence] [int] NOT NULL,
			[SupplierId] [int] NOT NULL,
		 CONSTRAINT [PK_DNA_CENTER] PRIMARY KEY CLUSTERED (
			[CenterNumber] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END

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