using AutoMapper;
using DNA.API;
using DNA.API.PluginTemplate.Models;
using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Services;
using DNA.Domain.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNA.API.PluginTemplate {
    [Service(typeof(IPluginStartupManager), Lifetime.Scoped)]
    public class PluginStartupManager : IPluginStartupManager {
        public string Name => nameof(PluginStartupManager).ToTitleCase();
        public string SourcePath => @"D:\Develop\DNA\Source\DNA.API\Plugins\DNA.API.PluginTemplate";
        public bool IsModule => false;

        private readonly ILogger<PluginStartupManager> _logger;
        private readonly IProcessService _processService;
        private readonly IMapper _mapper;

        public PluginStartupManager(ILogger<PluginStartupManager> logger, IProcessService processService, IMapper mapper) {
            _logger = logger;
            _processService = processService;
            _mapper = mapper;

#if DEBUG
            _logger.LogInformation($"ctor {nameof(PluginStartupManager)}");
#endif
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

        public JObject GetDefaultConfig(ConfigTemplate template) {

            ConfigProperty Config = template
                .RecurringJobsProperty(true, nameof(Services.PluginProcessJob))
                .Set("NoX", template.Property()
                    .Set("Document", template.Property())
                    )
                //.Set("EMails", template.Property()
                //    .SetEmail("Invoice", template.Property())
                //    )
                .Set("ELogoService", template.Property()
                    .Add("Enabled", true)
                    )
                .Set("EAdaptor", template.Property()
                    .Add("Enabled", true)
                    )
                ;

            template.SetConfigGenerated(true);

            var obj = JObject.FromObject(new {
                Worker = new {
                    Controllers = new {
                        Names = new string[] {
                            "Invoice",
                        },
                        //Invoice = new {
                        //    Hidden = false,
                        //    HiddenActions = new string[] { }
                        //}
                    }
                },
                Modules = new { },
                ConfigEditing = new {
                    Enabled = true,
                    Fields = template.GetFieldTemplates(""),
                    //AutoCompleteLists = template.AutoCompletes
                    //    .Add<Invoice>()
                    //    .Generate()
                },
                Config
            });

            return obj;
        }

        public List<ScreenModel> LoadModels() {
            return new List<ScreenModel> {
                //new ScreenModel(null, typeof(Invoice), true).Visibility(true).Emblem("receipt"),
            };
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
    }
}
