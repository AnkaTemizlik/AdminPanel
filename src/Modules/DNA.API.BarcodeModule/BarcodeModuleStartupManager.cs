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
    public class BarcodeModuleStartupManager : IPluginStartupManager {
        public string Name => nameof(BarcodeModuleStartupManager).ToTitleCase();
        public string SourcePath { get; set; } = @"D:\Develop\DNA\Source\DNA.API\Modules\DNA.API.BarcodeModule";
        public bool IsModule => true;
        public string MainModuleName => null;

        private readonly ILogger<BarcodeModuleStartupManager> _logger;

        public BarcodeModuleStartupManager(ILogger<BarcodeModuleStartupManager> logger) {
            _logger = logger;
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
                    Names = new string[] { "Barcode" },
                    Barcode = new {
                        Assembly = "DNA.API.BarcodeModule.dll"
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
