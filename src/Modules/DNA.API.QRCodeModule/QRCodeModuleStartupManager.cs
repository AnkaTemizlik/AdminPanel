using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Services;
using DNA.Domain.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DNA.API.Module {

    [Service(typeof(IPluginStartupManager), Lifetime.Scoped)]
    public class QRCodeModuleStartupManager : IPluginStartupManager {
        public string Name => nameof(QRCodeModuleStartupManager).ToTitleCase();
        public string SourcePath { get; set; } = @"D:\Develop\DNA\Source\DNA.API\Modules\DNA.API.QRCodeModule";
        public bool IsModule => true;

        private readonly ILogger<QRCodeModuleStartupManager> _logger;
        private readonly IConfiguration _configuration;

        public QRCodeModuleStartupManager(ILogger<QRCodeModuleStartupManager> logger, IConfiguration configuration) {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task DoWork() {
            try {
                await Task.CompletedTask;
            }
            catch { }
        }

        public List<ScreenModel> LoadModels() {
            return new List<ScreenModel> { };
        }

        public JObject GetDefaultConfig(ConfigTemplate template) {
            template.SetConfigGenerated(true);
            var obj = JObject.FromObject(new {
                Modules = new {
                    Names = new string[] { "QRCode" },
                    QRCode = new {
                        Assembly = "DNA.API.QRCodeModule.dll"
                    }
                }
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
            return null;
        }

        public void ApplyPluginMenus() { }
    }
}
