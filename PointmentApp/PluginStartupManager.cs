using AutoMapper;
using DNA.API;
using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Services;
using DNA.Domain.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PointmentApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PointmentApp {
    [Service(typeof(IPluginStartupManager), Lifetime.Scoped)]
    public class PluginStartupManager : IPluginStartupManager {
        public string Name => nameof(PluginStartupManager).ToTitleCase();
        public string SourcePath => @"D:\Develop\Anka\AnkaPanel\PointmentApp";
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
                await Task.CompletedTask;
            }
            catch (Exception ex) {
                _logger.LogError(AlertCodes.PluginStartupManagerError, ex);
            }
        }

        public JObject GetDefaultConfig(ConfigTemplate template) {

            ConfigProperty Config = template
                .RecurringJobsProperty(true, nameof(Services.AppProcessJob))
                ;

            template.SetConfigGenerated(true);

            var obj = JObject.FromObject(new {
                Worker = new {
                    Controllers = template.GenerateControllers(typeof(Controllers.AppointmentController))
                },
                Modules = new { },
                ConfigEditing = new {
                    Enabled = true,
                    Fields = template.GetFieldTemplates(""),
                    AutoCompleteLists = template.AutoCompletes
                        .Add<PriorityType>()
                        .Generate()
                },
                Config
            });

            return obj;
        }

        public List<ScreenModel> LoadModels() {
            return new List<ScreenModel> {
                new ScreenModel(null, typeof(Appointment), true).Visibility(true).Emblem("event_available").Editing(true),
                new ScreenModel(null, typeof(AppointmentEmployee), true).Visibility(true).Emblem("person_outline").Editing(true).HiddenInSidebar(),
                new ScreenModel(null, typeof(CustomerSummary), true).Visibility(true).Emblem("people_alt"),
                new ScreenModel(null, typeof(Document), true).Visibility(true).Emblem("image").HiddenInSidebar(),

                new ScreenModel(null, typeof(Service), true).Visibility(true).Emblem("cleaning_services").Definition().Editing(true),
                new ScreenModel(null, typeof(Customer), true).Visibility(true).Emblem("contacts").Definition().Editing(true),
                new ScreenModel(null, typeof(Country), true).Visibility(true).Emblem("outlined_flag").Definition().Editing(true),
                new ScreenModel(null, typeof(City), true).Visibility(true).Emblem("location_city").Definition().Editing(true),

                new ScreenModel(typeof(AppointmentState)),
                new ScreenModel(typeof(PriorityType)),
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
