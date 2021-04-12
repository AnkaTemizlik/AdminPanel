using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DNA.API.Extensions;
using DNA.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace DNA.API {
    /// <summary>
    /// 
    /// </summary>
    public class Program {
        public static async Task Main(string[] args) {

            var loggerFactory = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config");
            var logger = loggerFactory.GetCurrentClassLogger();
#if !DEBUG
            try {
#endif
            logger.Debug("initialize App");

            var configuration = GetConfig();

            ConfigureNLog(logger);

            Action<HostBuilderContext, IConfigurationBuilder> configureAppConfig = (context, builder) => {

                var config = builder.Build();
                IHostEnvironment env = context.HostingEnvironment;
                builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory); //env.ContentRootPath
                builder.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
                builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json".ToLower(), optional: true, reloadOnChange: true);
                builder.AddJsonFile($"appsettings.config.json", optional: false, reloadOnChange: true);
                builder.AddJsonFile($"appsettings.config.{env.EnvironmentName}.json".ToLower(), optional: true, reloadOnChange: true);

                // locals
                {
                    // tr
                    builder.AddJsonFile($"appsettings.locales.json", optional: true, reloadOnChange: true);
                    builder.AddJsonFile($"appsettings.locales.plugin.json", optional: true, reloadOnChange: true);

                    var section = config.GetSection("MultiLanguage");
                    var enabled = section.GetValue<bool?>("Enabled") ?? false;
                    if (enabled) {
                        var languages = section.GetSection("Languages").Get<string[]>();
                        foreach (var lng in languages) {
                            if (lng == "tr")
                                continue;
                            builder.AddJsonFile($"appsettings.locales.{lng}.json", optional: true, reloadOnChange: true);
                            builder.AddJsonFile($"appsettings.locales.plugin.{lng}.json", optional: true, reloadOnChange: true);
                        }
                    }
                }

                var files = config.GetSection("AdditionalSettingFiles").Get<string[]>();
                foreach (var file in files) {
                    builder.AddJsonFile(file, optional: false, reloadOnChange: true);
                }

                var names = config.GetSection("EditableConfigFiles:Names").Get<List<string>>();
                for (int i = 0; i < names.Count; i++) {
                    var s = config.GetSection($"EditableConfigFiles:Files:{i}");
                    builder.AddJsonFile(s.GetValue<string>("ConfigFile"), optional: false, reloadOnChange: true);
                }

                builder.AddEnvironmentVariables();
            };

            using var host = CreateHostBuilder(args)
                .ConfigureAppConfiguration(configureAppConfig)
                .Build();

            var workerEnabled = configuration.GetSection("Worker").GetValue<bool>("Enabled");
            if (!workerEnabled)
                logger.Warn("Arkaplan Hizmeti Kapalý. Timed Hosted Service is NOT ENABLED");

            await host.StartAsync();

            await host.WaitForShutdownAsync();
#if !DEBUG
            }
            catch (Exception exception) {
                //NLog: catch setup errors
                logger.Fatal(exception, "Stopped program because of exception");
                throw;
            }
            finally {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
#endif
        }

        public static void ConfigureNLog(NLog.Logger logger) {
            try {
                var configuration = GetConfig();
                var tablePrefix = configuration["Config:Database:TablePrefix"] ?? "DNA_";
                var logInsertQuery = configuration["Config:Database:LogInsertQuery"] ?? Persistence.Contexts.TableScripts.LogInsert;

                var databaseTarget = new NLog.Targets.DatabaseTarget("database");
                databaseTarget.ConnectionString = configuration.GetConnectionString("Default");
                databaseTarget.CommandText = logInsertQuery.Replace("{TablePrefix}", tablePrefix);
                databaseTarget.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@MachineName", "${machinename}"));
                databaseTarget.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Logged", @"${date:format=yyyy-MM-ddTHH\:mm\:ss.fff}"));
                databaseTarget.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Level", "${level}"));
                databaseTarget.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Message", "${message}"));
                databaseTarget.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Logger", "${logger}"));
                databaseTarget.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Callsite", "${callsite}"));
                databaseTarget.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@Exception", "${exception:tostring}"));
                databaseTarget.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@EntityName", "${var:EntityName}"));
                databaseTarget.Parameters.Add(new NLog.Targets.DatabaseParameterInfo("@EntityKey", "${var:EntityKey}"));

                /*
			    IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'DNA_NLOG')
			    BEGIN
				    SET ANSI_NULLS ON
				    SET QUOTED_IDENTIFIER ON
				    CREATE TABLE [dbo].[DNA_NLOG] (
					    [Id] [int] IDENTITY(1,1) NOT NULL,
					    [MachineName] [nvarchar](50) NOT NULL,
					    [Logged] [datetime] NOT NULL,
					    [Level] [nvarchar](50) NOT NULL,
					    [Message] [nvarchar](max) NOT NULL,
					    [Logger] [nvarchar](250) NULL,
					    [Callsite] [nvarchar](max) NULL,
					    [Exception] [nvarchar](max) NULL,
				    CONSTRAINT [PK_DNA_NLOG] PRIMARY KEY CLUSTERED ([Id] ASC)
				    WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
				    ) ON [PRIMARY]
			    END                
                 */

                /*    
                 * <target name="Mail" xsi:type="Mail" html="true" subject="Error Received" body="${message}"         
                     to="user2@someemaill.com"
                     from="user@someemail.com"
                     Encoding="UTF8"
                     smtpUsername="user@someemail.com"
                     enableSsl="False"
                     smtpPassword="pa$$word"
                     smtpAuthentication="Basic"
                     smtpServer="mail.someemail.com"
                     smtpPort="25" />
                 */
                //var errorMailConfig = configuration.GetSection("Config:LogSettings:ErrorEMail");
                //if (errorMailConfig.GetValue<bool>("Enabled")) {
                //    var mailTarget = new NLog.Targets.MailTarget("mail");
                //    mailTarget.To = errorMailConfig.GetValue<string>("To");
                //    mailTarget.Html = true;
                //    mailTarget.Subject = errorMailConfig.GetValue<string>("Subject");
                //    var errorMailBodyConfig = configuration.GetSection("Config:LogSettings:ErrorEMail:Body");
                //    mailTarget.Body = errorMailConfig.GetValue<string>("Subject");
                //}

                NLog.LogManager.Configuration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, databaseTarget, "*");

                var logPath = string.IsNullOrWhiteSpace(configuration["Logging:Dir"])
                    ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log")
                    : configuration["Logging:Dir"];

                var allfileTarget = (NLog.Targets.FileTarget)NLog.LogManager.Configuration.FindTargetByName("allfile");
                allfileTarget.FileName = Path.Combine(logPath, "nlog-all-${shortdate}.log");

                var ownFileTarget = (NLog.Targets.FileTarget)NLog.LogManager.Configuration.FindTargetByName("ownFile-web");
                allfileTarget.FileName = Path.Combine(logPath, "nlog-own-${shortdate}.log");

                NLog.LogManager.Configuration.Variables["internalLogFile"] = Path.Combine(logPath, "internal-nlog.txt");
                NLog.LogManager.ReconfigExistingLoggers();
            }
            catch (Exception ex) {
                logger.Error(ex, "ConfigureNLog return an error");
            }
        }

        public static IConfiguration GetConfig() {

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.config.json", true)
                .Build();

            return configuration;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {

            var configuration = GetConfig();

            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog() // NLog: Setup NLog for Dependency injection
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureWebHost(config => {
                    config.UseUrls(configuration["Origins:WEB"], configuration["Origins:Https"]);
                })
                .UseWindowsService()
                ;
        }
    }
}

