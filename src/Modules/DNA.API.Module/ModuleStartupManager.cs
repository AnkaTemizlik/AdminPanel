using DNA.API.Module.Jobs;
using DNA.API.Module.Services;
using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Services;
using DNA.Domain.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DNA.API.Module {

    [Service(typeof(IPluginStartupManager), Lifetime.Scoped)]
    public class ModuleStartupManager : IPluginStartupManager {
        public string Name => nameof(ModuleStartupManager).ToTitleCase();
        public string SourcePath { get; set; } = @"D:\Develop\DNA\Source\DNA.API\Modules\DNA.API.Module";
        public bool IsModule => true;

        private readonly ILogger<ModuleStartupManager> _logger;
        private readonly IModuleService _moduleService;
        private readonly IProcessService _processService;

        public ModuleStartupManager(ILogger<ModuleStartupManager> logger, IModuleService moduleService, IProcessService processService) {
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
                //new CreateTableModel("DNA_NoX_Document", typeof(Document), true, true, false),
                //new CreateTableModel(typeof(ERPModuleType))
            };
        }

        public JObject GetDefaultConfig(ConfigTemplate template) {
            ConfigProperty Config = template
                .RecurringJobsProperty(false, nameof(SynchronizationJob))
                //.Set("EMails", template.Property()
                //    .SetEmail("LicenseExpirationNotify", template.Property())
                //)
                ;

            template.SetConfigGenerated(true);

            var obj = JObject.FromObject(new {
                Modules = new {
                    Names = new string[] { "Module" },
                    Module = new {
                        Assembly = "DNA.API.Module.dll"
                    }
                },
                ConfigEditing = new {
                    Enabled = true,
                    Fields = template.GetFieldTemplates(""),
                    //AutoCompleteLists = template.AutoCompletes
                        //.Add<Notification>()
                        //.Generate()
                },
                Config
            });

            return obj;
        }

        public Dictionary<string, IEnumerable> GenerateScreenLists() {
            return new Dictionary<string, IEnumerable>();
        }

        public NotificationTypes GetNotificationTypes() {
            return new NotificationTypes();
        }

        public JObject GetScreenDefaults() {
            var file = Path.Combine(SourcePath, "DNA.API.Module.screens.json");
            if (File.Exists(file)) {
                return JObject.Parse(File.ReadAllText(file));
            }
            return null;
        }

        public void ApplyPluginMenus() {
            
        }
    }
}
