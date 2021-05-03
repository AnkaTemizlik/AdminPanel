﻿using AutoMapper;
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
        private readonly IMenuService _menuService;
        private readonly IProcessService _processService;
        private readonly IMapper _mapper;

        public PluginStartupManager(ILogger<PluginStartupManager> logger, IMenuService menuService, IProcessService processService, IMapper mapper) {
            _logger = logger;
            _menuService = menuService;
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

            _menuService
                .Root["panel"]
                .menus.Find(_ => _.name == "Settings")
                .menus.Find(_ => _.name == "Users")
                .visible = false;

            _menuService.Root["social"].menus
                .Add(new DNA.Domain.Models.Pages.Menu {
                    label = "WhatsApp",
                    to = "",
                    icon = "WhatsApp",
                    color = "#25d366",
                    areMenusVisible = true,
                    isHeaderVisible = true,
                    menus = new DNA.Domain.Models.Pages.MenuCollection()
                });

            ConfigProperty Config = template
                .RecurringJobsProperty(true, nameof(Services.AppProcessJob))
                .Set("SocialMediaLinks", false, template.Property()
                    .AddTextArea("Twitter", "https://twitter.com/")
                    .AddTextArea("YouTube", "https://www.youtube.com/channel/")
                    .AddTextArea("Facebook", "https://facebook.com/")
                    .AddTextArea("Instagram", "https://www.instagram.com/")
                    .AddTextArea("LinkedIn", "")
                    .AddTextArea("WhatsApp", "https://wa.me/9050000000?text=")
                    .AddTextArea("Email", "mailto:bilgi@?.com.tr")
                    )
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
                new ScreenModel(null, typeof(Appointment), true)
                    .Visibility(true)
                    .Editable(true)
                    .CalendarView("Title","AllDay", "StartDate", "EndDate", "Note",
                        new ScreenCalendarResource(typeof(Service), "ServiceId") { useColorAsDefault = true },
                        new ScreenCalendarResource(typeof(Customer), "CustomerId")
                     )
                    .Emblem("event_available"),
                new ScreenModel(null, typeof(AppointmentEmployee), true)
                    .Visibility(true)
                    .Emblem("person_outline")
                    .Editable(true)
                    .HiddenInSidebar(),
                new ScreenModel(null, typeof(CustomerSummary), true).Visibility(true).Emblem("people_alt"),
                new ScreenModel(null, typeof(Document), true)
                    .Visibility(true)
                    .GalleryView()
                    .Emblem("image").HiddenInSidebar(),

                new ScreenModel(null, typeof(Service), true).Visibility(true).Emblem("cleaning_services").Definition().Editable(true),
                new ScreenModel(null, typeof(Customer), true).Visibility(true).Emblem("contacts").Definition().Editable(true),
                new ScreenModel(null, typeof(Country), true).Visibility(true).Emblem("outlined_flag").Definition().Editable(true),
                new ScreenModel(null, typeof(City), true).Visibility(true).Emblem("location_city").Definition().Editable(true),

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

        public void ApplyPluginMenus() {

        }
    }
}
