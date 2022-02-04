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

namespace DNA.API.PdfModule {

    [Service(typeof(IPluginStartupManager), Lifetime.Scoped)]
    public class PdfModuleStartupManager : IPluginStartupManager {
        public string Name => nameof(PdfModuleStartupManager).ToTitleCase();
        public string SourcePath { get; set; } = @"D:\Develop\DNA\Source\DNA.API\Modules\DNA.API.PdfModule";
        public bool IsModule => true;
        public string MainModuleName => null;

        private readonly ILogger<PdfModuleStartupManager> _logger;

        public PdfModuleStartupManager(ILogger<PdfModuleStartupManager> logger) {
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
            ConfigProperty Config = template
                .Property()
                .Set("PdfSettings", template.Property()
                    .Add("PageWidth", 0)
                    .Add("PageHeight", 0)
                    .Set("Margins", template.Property()
                        .Add("Top", 0)
                        .Add("Right", 0)
                        .Add("Bottom", 0)
                        .Add("Left", 0)
                    )
                 );

            template.SetConfigGenerated(true);
            var obj = JObject.FromObject(new {
                Modules = new {
                    Names = new string[] { "Pdf" },
                    Module = new {
                        Assembly = "DNA.API.PdfModule.dll"
                    }
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
            return null;
        }

        public void ApplyPluginMenus() { }
    }
}
