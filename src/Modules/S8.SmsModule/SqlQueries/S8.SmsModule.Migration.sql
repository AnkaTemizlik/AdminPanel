/* PA_Sms ****************************/
IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('{TablePrefix}Sms' COLLATE SQL_Latin1_General_CP1_CI_AS) )
BEGIN
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON
	CREATE TABLE [{TablePrefix}Sms] (
		 [Id] INT NOT NULL IDENTITY(1, 1),
		 [CreationTime] DATETIME NOT NULL DEFAULT(GETDATE()),
		 [UpdateTime] DATETIME NOT NULL DEFAULT(GETDATE()),
		 [PhoneNumber] NVARCHAR(50) NOT NULL,
		 [Message] NVARCHAR(155) NOT NULL,
		 [SendTime] DATETIME NULL,
		 [Sender] NVARCHAR(50),
		 [InTurkish] BIT NULL,
		 [ScheduledSendingTime] DATETIME NULL,
		 [Sent] BIT NOT NULL DEFAULT(0),
		 [Response] NVARCHAR(1000),
	CONSTRAINT [PK_{TablePrefix}Sms] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
	SET ANSI_NULLS OFF
	SET QUOTED_IDENTIFIER OFF
END

/* PA_Sms UpdatedBy ****************************/
IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
	WHERE t.name = '{TablePrefix}Sms' AND c.name = 'Flags')
BEGIN
	ALTER TABLE [{TablePrefix}Sms] ADD Flags INT NOT NULL DEFAULT(0);
END
