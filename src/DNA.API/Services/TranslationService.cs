using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Services;
using DNA.Domain.Services.Communication;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(ITranslationService), Lifetime.Singleton)]
    public class TranslationService : ITranslationService {

        JObject _config;

        public TranslationService() {
            try {
                var ln = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                var fileName = $"appsettings.locales.plugin{(ln == "tr" ? "" : "." + ln)}.json";
                var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                if (!File.Exists(file))
                    file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.locales.plugin.json");
                JObject config;
                if (!File.Exists(file))
                    config = JObject.Parse("{Translations:{tr:{}}}");
                else
                    config = JObject.Parse(File.ReadAllText(file));
                _config = (JObject)config["Translations"];
            }
            finally { }
        }

        public string T(string key) {
            if (string.IsNullOrWhiteSpace(key))
                return key;
            if (_config == null)
                return key.ToTitleCase();
            var lng = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            if (_config[lng] == null)
                return key;
            return _config[lng].Value<string>(key.Trim()) ?? key.ToTitleCase();
        }

        //public Response T(Response response) {
        //    if (_config != null && !string.IsNullOrWhiteSpace(response.Message)) {
        //        var lng = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
        //        if (response.Message.Contains('|')) {
        //            var messages = response.Message.Split('|');
        //            var message = messages[0].Trim();
        //            var code = messages.Length > 0 ? messages[1].Trim() : "";
        //            response.Message = _config[lng].Value<string>(message) ?? message.ToTitleCase();
        //            response.Message = $"{response.Message} {code}";
        //        }
        //        else {
        //            response.Message = _config[lng].Value<string>(response.Message.Trim()) ?? response.Message.ToTitleCase();
        //        }
        //    }
        //    return response;

        //}
    }
}
