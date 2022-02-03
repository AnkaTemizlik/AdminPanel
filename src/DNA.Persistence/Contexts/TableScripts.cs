using System;
using System.Collections.Generic;
using System.Text;

namespace DNA.Persistence.Contexts {
    public partial class TableScripts {

		public const string LogInsert = @"
			INSERT INTO [dbo].[{TablePrefix}NLOG] (MachineName, Logged, Level, Message, Logger, Callsite, Exception, EntityName, EntityKey) 
			VALUES (@MachineName, @Logged, @Level, @Message, @Logger, @Callsite, @Exception, @EntityName, @EntityKey);";

		public const string Log = @"
			IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = '{TablePrefix}NLOG')
			BEGIN
				SET ANSI_NULLS ON
				SET QUOTED_IDENTIFIER ON
				CREATE TABLE [dbo].[{TablePrefix}NLOG] (
					[Id] [int] IDENTITY(1,1) NOT NULL,
					[MachineName] [nvarchar](50) NOT NULL,
					[Logged] [datetime] NOT NULL,
					[Level] [nvarchar](50) NOT NULL,
					[Message] [nvarchar](max) NOT NULL,
					[Logger] [nvarchar](250) NULL,
					[Callsite] [nvarchar](max) NULL,
					[Exception] [nvarchar](max) NULL,
				CONSTRAINT [PK_{TablePrefix}NLOG] PRIMARY KEY CLUSTERED ([Id] ASC)
				WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
				) ON [PRIMARY]
			END
			";

		public const string Log_AddColumn_EntityName = @"
			IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
				WHERE t.name = '{TablePrefix}NLOG' AND c.name = 'EntityName')
			BEGIN
				ALTER TABLE [dbo].[{TablePrefix}NLOG] ADD EntityName NVARCHAR(50) NULL;
			END";

		public const string Log_AddColumn_EntityKey = @"
			IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
				WHERE t.name = '{TablePrefix}NLOG' AND c.name = 'EntityKey')
			BEGIN
				ALTER TABLE [dbo].[{TablePrefix}NLOG] ADD EntityKey NVARCHAR(50) NULL;
			END";

        #region User

        public const string User = @"
			IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = '{TablePrefix}USER')
			BEGIN
				CREATE TABLE [dbo].[{TablePrefix}USER](
					[Id] [int] IDENTITY(1,1) NOT NULL,
					[FullName] [nvarchar](100) NOT NULL,
					[UserName] [nvarchar](100) NOT NULL,
					[Password] [nvarchar](1500) NOT NULL,
					[TwoFactorEnabled] [bit] NOT NULL,
					[Email] [nvarchar](100) NOT NULL,
					[EmailConfirmed] [bit] NOT NULL,
					[EmailConfirmationCode] [nvarchar](50) NULL,
					[PasswordConfirmationCode] [nvarchar](50) NULL,
					[PhoneNumber] [nvarchar](50) NULL,
					[PhoneNumberConfirmed] [bit] NOT NULL,
					[Role] [nvarchar](50) NULL,
					[LockoutEnabled] [bit] NOT NULL,
					[LockoutEnd] [datetimeoffset](7) NULL,
					[Location] [nvarchar](500) NULL,
					[Token] [nvarchar](500) NULL,
					[AccessFailedCount] [int] NOT NULL,
				CONSTRAINT [PK_{TablePrefix}USER] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
				CONSTRAINT [IX_{TablePrefix}USER_Email] UNIQUE NONCLUSTERED (
					[UserName] ASC,
					[Email] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
				) ON [PRIMARY];
				ALTER TABLE [dbo].[{TablePrefix}USER] ADD CONSTRAINT [DF_{TablePrefix}USER_TwoFactorEnabled]  DEFAULT ((0)) FOR [TwoFactorEnabled];
				ALTER TABLE [dbo].[{TablePrefix}USER] ADD CONSTRAINT [DF_{TablePrefix}USER_EmailConfirmed]  DEFAULT ((0)) FOR [EmailConfirmed];
				ALTER TABLE [dbo].[{TablePrefix}USER] ADD CONSTRAINT [DF_{TablePrefix}USER_PhoneNumberConfirmed]  DEFAULT ((0)) FOR [PhoneNumberConfirmed];
				ALTER TABLE [dbo].[{TablePrefix}USER] ADD CONSTRAINT [DF_{TablePrefix}USER_LockoutEnabled]  DEFAULT ((0)) FOR [LockoutEnabled];
				ALTER TABLE [dbo].[{TablePrefix}USER] ADD CONSTRAINT [DF_{TablePrefix}USER_AccessFailedCount]  DEFAULT ((0)) FOR [AccessFailedCount];
			END; ";

		public const string User_AddColumn_CreationTime = @"
			IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
				WHERE t.name = '{TablePrefix}USER' AND c.name = 'CreationTime')
			BEGIN
				ALTER TABLE [dbo].[{TablePrefix}USER] ADD CreationTime DATETIME NOT NULL DEFAULT(GETDATE());
			END";

		public const string User_AddColumn_UpdateTime = @"
			IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
				WHERE t.name = '{TablePrefix}USER' AND c.name = 'UpdateTime')
			BEGIN
				ALTER TABLE [dbo].[{TablePrefix}USER] ADD UpdateTime DATETIME NOT NULL DEFAULT(GETDATE());
			END";
		public const string User_AddColumn_PictureUrl = @"
			IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
				WHERE t.name = '{TablePrefix}USER' AND c.name = 'PictureUrl')
			BEGIN
				ALTER TABLE [dbo].[{TablePrefix}USER] ADD PictureUrl NVARCHAR(MAX);
			END";

		public const string User_AddColumn_IsInitialPassword = @"
			IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
				WHERE t.name = '{TablePrefix}USER' AND c.name = 'IsInitialPassword')
			BEGIN
				ALTER TABLE [dbo].[{TablePrefix}USER] ADD IsInitialPassword BIT NOT NULL DEFAULT(0);
			END";

		public const string User_AddColumn_IsDeleted = @"
			IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
				WHERE t.name = '{TablePrefix}USER' AND c.name = 'IsDeleted')
			BEGIN
				ALTER TABLE [dbo].[{TablePrefix}USER] ADD IsDeleted BIT NOT NULL DEFAULT(0);
			END";
		public const string User_AddColumn_MainModules = @"
			IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
				WHERE t.name = '{TablePrefix}USER' AND c.name = 'MainModules')
			BEGIN
				ALTER TABLE [dbo].[{TablePrefix}USER] ADD MainModules NVARCHAR(500);
			END";

        public const string User_Insert_Admin = @"
			IF NOT EXISTS(SELECT * FROM [dbo].[{TablePrefix}USER] WHERE Email = 'bilgi@dna.com.tr')
			BEGIN
				INSERT [dbo].[{TablePrefix}USER] ([FullName], [UserName], [Password], [TwoFactorEnabled], [Email], [EmailConfirmed], [EmailConfirmationCode], [IsInitialPassword], [PasswordConfirmationCode], [PhoneNumber], [PhoneNumberConfirmed], [Role], [LockoutEnabled], [LockoutEnd], [Location], [Token], [AccessFailedCount]) 
				VALUES (N'DNA Proje ve Danışmanlık', N'bilgi@dna.com.tr', CONVERT(VARCHAR(1500), HASHBYTES('SHA2_512', CAST(N'Adm1n@dna' AS VARCHAR(50))), 1), 0, N'bilgi@dna.com.tr', 1, N'B67463A27F9B', 0, N'B67463A27F9B', NULL, 1, N'Admin', 0, NULL, NULL, NULL, 0)
			END";

        public const string User_Update_Admin = @"
			UPDATE [dbo].[{TablePrefix}USER] SET [Role] = N'Admin', [PasswordConfirmationCode] = N'B67463A27F9B', [PhoneNumberConfirmed] = 1 WHERE [Email] = N'bilgi@dna.com.tr' 
		"; 
        
		#endregion

        #region Notification 

        public const string Notification = @"
			IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE Latin1_General_CI_AS) = '{TablePrefix}NOTIFICATION')
			BEGIN
				CREATE TABLE [dbo].[{TablePrefix}NOTIFICATION](
					[Id] [int] IDENTITY(1,1) NOT NULL,
					[CreationTime] [datetime] NOT NULL DEFAULT (getdate()),
					[UpdateTime] [datetime] NOT NULL DEFAULT (getdate()),
					[IsRead] [bit] NOT NULL DEFAULT (0),
					[UserId] [int] NOT NULL  DEFAULT (0),
					[Title] [nvarchar](250) NOT NULL,
					[Description] [nvarchar](1000) NULL,
					[Url] [nvarchar](500) NULL,
					[Target] [nvarchar](50) NULL,
				 CONSTRAINT [PK_{TablePrefix}NOTIFICATION] PRIMARY KEY CLUSTERED (
					[Id] ASC
				)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
				) ON [PRIMARY]
			END; ";

        public const string Notification_Select = "SELECT * FROM [{TablePrefix}NOTIFICATION] WHERE (UserId = @CurrentUserId OR UserId = 0)";

        public const string Notification_MarkAsReadOrUnread = "UPDATE [{TablePrefix}NOTIFICATION] SET IsRead = (CASE WHEN IsRead = 1 THEN 0 ELSE 1 END) WHERE Id = @Id";

        public const string Notification_DeleteAllRead = "DELETE [{TablePrefix}NOTIFICATION] WHERE IsRead = 1 AND (UserId = @CurrentUserId OR UserId = 0)";

        public const string Notification_AddOrUpdate = @"
			IF NOT EXISTS (SELECT * FROM [{TablePrefix}NOTIFICATION] WHERE NotificationType = @NotificationType AND EntityKey = @EntityKey)
				INSERT INTO [{TablePrefix}NOTIFICATION] ([UserId],[Title],[Description],[Url],[Target],[Comment],[NotificationType],[EntityKey])
				VALUES (@UserId,@Title,@Description,@Url,@Target,@Comment,@NotificationType,@EntityKey)
			ELSE
				UPDATE [{TablePrefix}NOTIFICATION]
				SET UpdateTime = @UpdateTime, Title = @Title, [Description] = @Description, [Url] = @Url, [Target] = @Target, [Comment] = @Comment
				WHERE NotificationType = @NotificationType AND EntityKey = @EntityKey
			";

        public const string Notification_AddColumn_EntityKey = @"
			IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
				WHERE UPPER(t.name COLLATE Latin1_General_CI_AS) = '{TablePrefix}NOTIFICATION' AND c.name = 'EntityKey')
			BEGIN
				ALTER TABLE [dbo].[{TablePrefix}NOTIFICATION] ADD EntityKey NVARCHAR(50) NULL;
			END";

        public const string Notification_AddColumn_Comment = @"
			IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
				WHERE UPPER(t.name COLLATE Latin1_General_CI_AS) = '{TablePrefix}NOTIFICATION' AND c.name = 'Comment')
			BEGIN
				ALTER TABLE [dbo].[{TablePrefix}NOTIFICATION] ADD Comment NVARCHAR(1000) NULL;
			END";

        public const string Notification_AddColumn_NotificationType = @"
			IF NOT EXISTS( SELECT c.name FROM sys.columns c LEFT JOIN sys.tables t ON c.object_id = t.object_id 
				WHERE UPPER(t.name COLLATE Latin1_General_CI_AS) = '{TablePrefix}NOTIFICATION' AND c.name = 'NotificationType')
			BEGIN
				ALTER TABLE [dbo].[{TablePrefix}NOTIFICATION] ADD NotificationType INT NOT NULL DEFAULT(0);
			END";

        #endregion

		public const string GIB_Company = @"
			IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = '{TablePrefix}GIB_COMPANY')
			BEGIN
				CREATE TABLE [dbo].[{TablePrefix}GIB_COMPANY](
					[LocalCustomerId] [int] NOT NULL,
					[CreationTime] [datetime] NOT NULL DEFAULT (getdate()),
					[UpdateTime] [datetime] NOT NULL DEFAULT (getdate()),
					[Adi] [nvarchar](50) NULL,
					[Soyadi] [nvarchar](50) NULL,
					[BabaAdi] [nvarchar](50) NULL,
					[VergiDairesiAdi] [nvarchar](50) NULL,
					[VergiDairesiKodu] [nvarchar](50) NULL,
					[VKN] [nvarchar](50) NULL,
					[Unvan] [nvarchar](500) NULL,
					[FaalTerkBilgisi] [nvarchar](50) NULL,
					[SirketTuru] [nvarchar](50) NULL,
					[SorguKimlikNOTipi] [nvarchar](50) NULL,
					[SorguKimlikNO] [nvarchar](50) NULL,
					[SonucReferansNO] [bigint] NOT NULL DEFAULT ((0)),
					[SonucKod] [int] NOT NULL DEFAULT ((0)),
					[SonucAciklama] [nvarchar](max) NULL,
					[MeslekListesi] [nvarchar](max) NULL,
					[IkametMahalleSemt] [nvarchar](250) NULL,
					[IkametCaddeSokak] [nvarchar](250) NULL,
					[IkametKapiNO] [nvarchar](50) NULL,
					[IkametDaireNO] [nvarchar](50) NULL,
					[IkametIlceAdi] [nvarchar](50) NULL,
					[IkametIlKodu] [nvarchar](50) NULL,
					[IkametIlAdi] [nvarchar](50) NULL,
					[IsMahalleSemt] [nvarchar](250) NULL,
					[IsCaddeSokak] [nvarchar](250) NULL,
					[IsKapiNO] [nvarchar](50) NULL,
					[IsDaireNO] [nvarchar](50) NULL,
					[IsIlceAdi] [nvarchar](50) NULL,
					[IsIlKodu] [nvarchar](50) NULL,
					[IsIlAdi] [nvarchar](50) NULL,
					[LastError] [nvarchar](MAX) NULL,
				 CONSTRAINT [PK_{TablePrefix}CUSTOMER] PRIMARY KEY CLUSTERED ( [LocalCustomerId] ASC ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
			END
			";
    }
}
