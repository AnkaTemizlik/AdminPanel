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
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(IPluginStartupManager), Lifetime.Scoped)]
    public class APIStartupManager : IPluginStartupManager {

        private readonly IConfiguration _configuration;
        private readonly ILogger<APIStartupManager> _logger;

        public string Name => nameof(APIStartupManager).ToTitleCase();
        public string SourcePath => AppDomain.CurrentDomain.BaseDirectory;
        public bool IsModule => true;
        public string MainModuleName => null;
        public APIStartupManager(IConfiguration configuration, ILogger<APIStartupManager> logger) {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task DoWork() {

            // TODO: Check License
            // var url = _configuration["Config:Guard:Url"];
            // var key = _configuration["Config:Guard:ClientSecret"];
            // var workOffline = _configuration.GetSection("Config:Guard").GetValue<bool>("WorkOffline");
            // var checkLicenseResponse = await _licenseService.CheckLicenseAsync(url, key, workOffline);

            // _logger.Log(checkLicenseResponse);

            await Task.CompletedTask;
        }

        public List<ScreenModel> LoadModels() {
            return new List<ScreenModel>();
        }

        public JObject GetDefaultConfig(ConfigTemplate template) {
            return null;
        }

        public Dictionary<string, IEnumerable> GenerateScreenLists() {
            var list = new Dictionary<string, IEnumerable>();
            list.Add("DateFormats", GetDateFormats());
            return list;
        }

        public NotificationTypes GetNotificationTypes() {
            return new NotificationTypes();
        }

        List<dynamic> GetDateFormats() {
            var dateFormats = new List<dynamic>();
            dateFormats.Add(new {
                localized = "LT",
                format = new {
                    tr = "HH:mm",
                    en = "h:mm a"
                },
                caption = "18:25"
            });

            dateFormats.Add(new {
                localized = "LTS",
                format = new {
                    tr = "HH:mm:ss",
                    en = "h:mm:ss a"
                },
                caption = "18:25:03"
            });

            dateFormats.Add(new {
                localized = "L",
                format = new {
                    tr = "dd.MM.yyyy",
                    en = "MM/dd/YYYY"
                },
                caption = "02.01.2021"
            });

            dateFormats.Add(new {
                localized = "l",
                format = new {
                    tr = "d.M.yy",
                    en = "M/d/yy"
                },
                caption = "2.1.21"
            });

            dateFormats.Add(new {
                localized = "LL",
                format = new {
                    tr = "d MMMM yyyy",
                    en = "MMMM d yyyy"
                },
                caption = "2 January 2021"
            });

            dateFormats.Add(new {
                localized = "ll",
                format = new {
                    tr = "d MMM yy",
                    en = "MMM d yy"
                },
                caption = "2 Jan 21"
            });

            dateFormats.Add(new {
                localized = "LLL",
                format = new {
                    tr = "dd.MM.yyyy HH:mm",
                    en = "MM/dd/yyyy h:mm a"
                },
                caption = "02.01.2021 18:25"
            });

            dateFormats.Add(new {
                localized = "lll",
                format = new {
                    tr = "d.M.yy H:m",
                    en = "M/d/yy h:mm a"
                },
                caption = "2.1.21 18:25"
            });

            dateFormats.Add(new {
                localized = "LLLL",
                format = new {
                    tr = "dd.MM.yyyy HH:mm:ss.SSS",
                    en = "dddd, MMMM yyyy h:mm:ss.SSS a"
                },
                caption = "02.01.2021 18:25:03.313"
            });

            dateFormats.Add(new {
                localized = "llll",
                format = new {
                    tr = "dd.M.yy H:m:ss.SS",
                    en = "ddd, MMM yy h:mm:ss.SSS a"
                },
                caption = "02.1.21 18:25:03.31"
            });
            return dateFormats;
        }

        public JObject GetScreenDefaults() {
            return null;
        }

        public void ApplyPluginMenus() {
            
        }
    }
}
