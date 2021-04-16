using DNA.Domain.Models;
using DNA.Domain.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Domain.Services {
    public interface IWritableOptions {
        Task<dynamic> Update(int fileId, Dictionary<string, dynamic> changes);
        Task<dynamic> Get(bool isAuthenticated);
        Task<JObject> GetScreenConfig();
        Task<dynamic> GetLocalesConfigAsync();

        void GenerateConfigs();
        ConfigTemplate GetConfigTemplate();
    }
}
