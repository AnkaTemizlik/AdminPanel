using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Services;
using DNA.Domain.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using S8.SmsModule.Models;
using S8.SmsModule.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace S8.SmsModule {
    [Service(typeof(IPluginStartupManager), Lifetime.Scoped)]
    public class SmsModuleStartupManager : IPluginStartupManager {
        public string Name => nameof(SmsModuleStartupManager).ToTitleCase();
        public string SourcePath { get; set; } = @"D:\Develop\DNA\Source\DNA.API\Modules\S8.SmsModule";
        public bool IsModule => true;

        private readonly ILogger<SmsModuleStartupManager> _logger;
        private readonly ISmsService _moduleService;
        private readonly IProcessService _processService;

        public SmsModuleStartupManager(ILogger<SmsModuleStartupManager> logger, ISmsService moduleService, IProcessService processService) {
            _logger = logger;
            _moduleService = moduleService;
            _processService = processService;
        }

        public async Task DoWork() {
            try {
                await _processService.ExecuteAsync(SqlQueries.Migration);
                _logger.LogInformation(AlertCodes.PluginStartupManagerInfo);
            }
            catch (Exception ex) {
                _logger.LogError(AlertCodes.PluginStartupManagerError, ex);
            }
        }

        public List<ScreenModel> LoadModels() {
            return new List<ScreenModel> {
                new ScreenModel(typeof(Sms), true)
                    .Visibility(true)
                    .Emblem("sms")
                    .Editable(true)
                    .HiddenInSidebar()
            };
        }

        public JObject GetDefaultConfig(ConfigTemplate template) {
            ConfigProperty Config = template
                .RecurringJobsProperty(true, nameof(Jobs.SmsSyncJob))
                .Set("SmsSettings", true, template.Property()
                    .Add("ActiveProviderName", "VatanSMS")
                    .Set("VatanSMS", false, template.Property()
                        .Add("RemoteAddress", "https://www.oztekbayi.com/webservis/service.php")
                        .Add("CustomerCode", "")
                        .Add("UserName", "")
                        .AddPassword("Password", "")
                        .Add("Originator", "")
                        .Add("SendTimeFormat", "yyyy-MM-dd HH:mm:ss")
                        .Add("CancelationLink", "İptal için https://...")
                        )
                    )
                ;

            template.SetConfigGenerated(true);

            var obj = JObject.FromObject(new {
                Modules = new {
                    Names = new string[] { "Sms" },
                    Module = new {
                        Assembly = "S8.SmsModule.dll"
                    }
                },
                ConfigEditing = new {
                    Enabled = true,
                    Fields = template.GetFieldTemplates(""),
                    AutoCompleteLists = template.AutoCompletes
                        .Add<Sms>()
                        .Generate()
                },
                Config
            });

            return obj;
        }

        public Dictionary<string, IEnumerable> GenerateLists() {
            return new Dictionary<string, IEnumerable>();
        }

        public NotificationTypes GetNotificationTypes() {
            return new NotificationTypes();
        }

        public JObject GetScreenDefaults() {
            //var file = Path.Combine(SourcePath, "S8.SmsModule.screens.json");
            //if (File.Exists(file)) {
            //    return JObject.Parse(File.ReadAllText(file));
            //}
            return null;
        }

        public Dictionary<string, IEnumerable> GenerateScreenLists() {
            return new Dictionary<string, IEnumerable>();
        }

        public void ApplyPluginMenus() {

        }
    }
}
