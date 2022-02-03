using DNA.Domain.Models;
using DNA.Domain.Services.Communication;
using DNA.Domain.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNA.Domain.Services {
    public interface IPluginStartupManager {
        string Name { get; }
        string SourcePath { get; }
        string MainModuleName { get; }
        bool IsModule { get; }
        Task DoWork();
        List<ScreenModel> LoadModels();
        JObject GetDefaultConfig(ConfigTemplate template);
        Dictionary<string, System.Collections.IEnumerable> GenerateScreenLists();
        NotificationTypes GetNotificationTypes();
        JObject GetScreenDefaults();
        void ApplyPluginMenus();
    }
}
