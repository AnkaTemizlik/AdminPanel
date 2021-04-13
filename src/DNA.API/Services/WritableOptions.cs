using DNA.Domain.Models;
using DNA.Domain.Services;
using DNA.Domain.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DNA.Domain.Extentions;
using Microsoft.Extensions.DependencyInjection;

namespace DNA.API.Services {

    public class WritableOptions : IWritableOptions {

        //private readonly IWebHostEnvironment _environment;
        private readonly IConfigurationRoot _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly JsonMergeSettings _jsonMergeSettings = new JsonMergeSettings {
            MergeArrayHandling = MergeArrayHandling.Union,
            MergeNullValueHandling = MergeNullValueHandling.Merge
        };

        public ConfigTemplate GetConfigTemplate() => new ConfigTemplate();

        public WritableOptions(IConfigurationRoot configuration, IServiceProvider serviceProvider) {
            //_environment = environment;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public async Task<JObject> GetScreenConfig() {
            //var fileName = _configuration["ScreenConfigFiles:PluginScreens:ConfigFile"];
            var fileName = _configuration.GetSection("AdditionalSettingFiles").Get<string[]>().FirstOrDefault();
            var file = fileName ?? "";
            if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(file)))
                file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
            if (File.Exists(file)) {
                var json = await File.ReadAllTextAsync(file);
                var jObject = Newtonsoft.Json.Linq.JObject.Parse(json);
                var names = new List<string>();
                foreach (var item in (jObject["ScreenConfig"]["Names"] as JArray)) {
                    var screen = jObject["ScreenConfig"]["Screens"][item.ToString()];
                    if (screen != null) {
                        if (screen["visible"] != null) {
                            var visible = (bool)(screen["visible"] as JValue).Value;
                            if (!visible)
                                jObject["ScreenConfig"]["Screens"][item.ToString()] = null;
                            else
                                names.Add(item.ToString());
                        }
                    }
                }
                jObject["ScreenConfig"]["Names"] = JArray.FromObject(names);
                return jObject["ScreenConfig"] as JObject;
            }
            else {
                return new JObject(new { names = new { } });
            }
        }

        public async Task<dynamic> GetLocalesConfigAsync(string culture, string ns) {

            var section = _configuration.GetSection("MultiLanguage");
            var enabled = section.GetValue<bool?>("Enabled") ?? false;
            if (enabled) {
                culture = string.IsNullOrWhiteSpace(culture) ? string.Empty : culture;
                if (culture.Contains('-'))
                    culture = culture.Split('-')[0];
            }
            else {
                culture = section.GetValue<string>("Default") ?? "tr";
            }

            ns = ns == "common" ? string.Empty : $"{ns}.";
            var file = $"appsettings.locales.{ns}{(culture == "tr" ? string.Empty : $"{culture}.")}json";
            file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);

            if (File.Exists(file)) {
                file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
                var json = await File.ReadAllTextAsync(file);
                var jObject = Newtonsoft.Json.Linq.JObject.Parse(json);
                return jObject["Translations"][culture];
            }

            return new { };
        }

        public async Task<dynamic> Get() {
            var names = _configuration.GetSection("EditableConfigFiles:Names").Get<List<string>>();
            var s = _configuration.GetSection($"EditableConfigFiles:Files:0");
            //var list = new dynamic[names.Count];
            //for (int i = 0; i < names.Count; i++) {
            //    var s = _configuration.GetSection($"EditableConfigFiles:Files:{i}");
            //    var c = await GetConfig(i, s.GetValue<string>("Title"), s.GetValue<string>("Icon"), s.GetValue<string>("ConfigFile"));
            //    list[i] = c;
            //}
            return await GetConfig(0, s.GetValue<string>("Title"), s.GetValue<string>("Icon"), s.GetValue<string>("ConfigFile"));
        }

        async Task<dynamic> GetConfig(int id, string title, string icon, string fileName) {

            string file = null;
            if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(fileName)))
                file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

            if (string.IsNullOrWhiteSpace(file))
                return new {
                    id,
                    title,
                    icon,
                    values = new JObject(),
                    config = new JObject(),
                    version
                };

            var json = await File.ReadAllTextAsync(file);
            var jObject = Newtonsoft.Json.Linq.JObject.Parse(json);

            foreach (var item in jObject.SelectToken("ConfigEditing.Fields").Children()) {

                var visibleValue = ((Newtonsoft.Json.Linq.JProperty)item).Value["visible"];
                bool visible = visibleValue == null ? true : (bool)((Newtonsoft.Json.Linq.JValue)visibleValue).Value;
                if (!visible) {
                    var path = ((Newtonsoft.Json.Linq.JProperty)item).Name.Replace(":", ".");
                    var config = jObject.SelectToken("Config." + path);
                    if (config != null)
                        config.Parent.Remove();
                }

                var typeValue = ((Newtonsoft.Json.Linq.JProperty)item).Value["type"];
                bool password = typeValue == null ? false : ((Newtonsoft.Json.Linq.JValue)typeValue).Value.ToString() == "password";
                if (password) {
                    var path = ((Newtonsoft.Json.Linq.JProperty)item).Name.Replace(":", ".");
                    var config = jObject.SelectToken("Config." + path);
                    if (config != null) {
                        var parent = (Newtonsoft.Json.Linq.JProperty)config.Parent;
                        parent.Value = "********";
                    }
                }
            }

            //if (forMergeConfig != null)
            //    forMergeConfig.Merge(jObject, new JsonMergeSettings {
            //        MergeArrayHandling = MergeArrayHandling.Merge,
            //        MergeNullValueHandling = MergeNullValueHandling.Merge
            //    });
            //else
            //    forMergeConfig = jObject;

            return new {
                id,
                title,
                icon,
                values = jObject["Config"],
                config = jObject["ConfigEditing"],
                version
            };
        }

        public async Task<dynamic> Update(int fileId, Dictionary<string, dynamic> changes) {
            var section = _configuration.GetSection($"EditableConfigFiles:Files:{fileId}");
            var fileName = section.GetValue<string>("ConfigFile");
            var file = fileName;

            if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(file))) {
                file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
            }

            var json = await File.ReadAllTextAsync(file);
            var jObject = Newtonsoft.Json.Linq.JObject.Parse(json);
            foreach (var item in changes) {
                var fields = item.Key.Split(":");
                var value = item.Value;
                if (fields.Length == 1)
                    jObject["Config"][fields[0]] = JToken.FromObject(value);
                else if (fields.Length == 2)
                    jObject["Config"][fields[0]][fields[1]] = JToken.FromObject(value);
                else if (fields.Length == 3)
                    jObject["Config"][fields[0]][fields[1]][fields[2]] = JToken.FromObject(value);
                else if (fields.Length == 4)
                    jObject["Config"][fields[0]][fields[1]][fields[2]][fields[3]] = JToken.FromObject(value);
                else if (fields.Length == 5)
                    jObject["Config"][fields[0]][fields[1]][fields[2]][fields[3]][fields[4]] = JToken.FromObject(value);
                else if (fields.Length == 6)
                    jObject["Config"][fields[0]][fields[1]][fields[2]][fields[3]][fields[4]][fields[5]] = JToken.FromObject(value);
            }
            File.WriteAllText(file, JsonConvert.SerializeObject(jObject, Formatting.Indented), System.Text.Encoding.UTF8);
            _configuration.Reload();
            return await Get();
        }

        public void GenerateConfigs() {
            try {
                var tablePrefix = _configuration["Config:Database:TablePrefix"];
                var models = new ScreenModelCollection(tablePrefix);

                models.TryAdd(new ScreenModel(null, typeof(Notification), true).HiddenInSidebar());
                models.TryAdd(new ScreenModel(null, typeof(Models.User), true).HiddenInSidebar());
                models.TryAdd(new ScreenModel(typeof(LogLevelTypes)));
                models.TryAdd(new ScreenModel(typeof(OperatorTypes)));

                string pluginSourcePath = null;
                var pluginConfigTemplate = JObject.Parse("{}");
                var moduleScreenDefaults = JObject.Parse("{}");
                var extraScreenLists = new Dictionary<string, IEnumerable>();
                var pluginNotifyTypes = new NotificationTypes();
                pluginNotifyTypes.AddDefaults();

                extraScreenLists.Add("NotificationTypes", pluginNotifyTypes);

                var provider = _serviceProvider.CreateScope().ServiceProvider;
                foreach (var manager in provider.GetServices<IPluginStartupManager>().OrderBy(_ => !_.IsModule)) {
                    // load models
                    models.TryAddRange(manager.LoadModels());
                    // add notify types
                    pluginNotifyTypes.AddRange(manager.GetNotificationTypes());
                    // add extra list to screen config
                    var pluginExtraLists = manager.GenerateScreenLists();
                    if (pluginExtraLists != null) {
                        foreach (var item in pluginExtraLists) {
                            if (!extraScreenLists.ContainsKey(item.Key)) {
                                extraScreenLists[item.Key] = item.Value;
                            }
                        }
                    }

                    if (manager.IsModule) {
                        pluginConfigTemplate.Merge(manager.GetDefaultConfig(new ConfigTemplate()), _jsonMergeSettings);
                        moduleScreenDefaults.Merge(manager.GetScreenDefaults(), _jsonMergeSettings);
                    }
                    else {
                        if (!manager.SourcePath.Contains(":"))
                            throw new Exception("PluginSourcePath tam yolu belirtmeli: " + manager.Name);
                        pluginConfigTemplate.Merge(manager.GetDefaultConfig(new ConfigTemplate()), _jsonMergeSettings);
                        pluginSourcePath = manager.SourcePath;
                    }
                }

                ConfigMigration(pluginSourcePath, pluginConfigTemplate, models.Select(_ => _.Type).ToArray());

                if (models.Count > 0) {
#if DEBUG
                    SqlTableScriptGenerator.Generate(pluginSourcePath, models, tablePrefix);
#endif
                    AddModelsToPluginScreenConfig(pluginSourcePath, models, extraScreenLists, moduleScreenDefaults);
                    AddModelsToTranslation(pluginSourcePath, models);
                }

                _configuration.Reload();
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /*
        [Obsolete]
        void GenerateConfigX(IPluginStartupManager manager) {
            if (manager.IsModule)
                return;
            if (!manager.PluginSourcePath.Contains(":"))
                throw new Exception("PluginSourcePath tam yolu belirtmeli.");

            var pluginSourcePath = manager.IsModule ? null : manager.PluginSourcePath;
            var models = manager.LoadModels();
            var pluginConfigTemplate = manager.GetDefaultConfig(new ConfigTemplate());
            var pluginExtraLists = manager.GenerateLists();
            var pluginNotifyTypes = manager.GetNotificationTypes() ?? new NotificationTypes();
            pluginNotifyTypes.AddDefaults();

            if (!models.Any(_ => _.Name == typeof(Notification).Name))
                models.Add(new ScreenModel(null, typeof(Notification), true, true, false));
            if (!models.Any(_ => _.Name == typeof(Models.User).Name))
                models.Add(new ScreenModel(null, typeof(Models.User), false, true, false));
            if (!models.Any(_ => _.Name == typeof(LogLevelTypes).Name))
                models.Add(new ScreenModel(typeof(LogLevelTypes)));
            if (!models.Any(_ => _.Name == typeof(OperatorTypes).Name))
                models.Add(new ScreenModel(typeof(OperatorTypes)));

            var extraScreenLists = new Dictionary<string, IEnumerable>();
            if (pluginExtraLists != null) {
                foreach (var item in pluginExtraLists) {
                    if (!extraScreenLists.ContainsKey(item.Key)) {
                        extraScreenLists[item.Key] = item.Value;
                    }
                }
            }
            extraScreenLists.Add("NotificationTypes", pluginNotifyTypes);
        }
        */

        void AddModelsToPluginScreenConfig(string pluginSourcePath, ScreenModelCollection list, Dictionary<string, IEnumerable> extraLists, JObject moduleScreenDefaults) {

            #region sample
            /*

    Preloading: [
          { "name": "Module" }, 
          ...

     Screens: {
        Notification: {
            "title": "Notifications",
            "route": "notification",
            "keyFieldName": "Id",
            "hideInSidebar": true,
            "icon": "notifications",
            "rowAlternationEnabled": false,
            "isDefinitionModel": false,
            "editing": {
              "enabled": true,
              "mode": "popup",
              "allowUpdating": true,
              "allowAdding": true,
              "allowDeleting": true
            },
            "onRowPrepared": {
              "formating": {
                "field": "IsRead",
                "value": true,
                "style": {
                  "backgroundColor": "#43a0474d"
                }
              }
            },
            "actions": [
                {
                    "text": "Goto the License",
                    "icon": "security",
                    "route": "/panel/screen/license/{EntityKey}"
                },
                {
                    "text": "Mark as Read",
                    "icon": "mark_email_read",
                    "dependsOnSelected": false
                    "request": {
                        "method": "PUT",
                        "url": "api/notification/mark-as-read/{Id}",
                        "data": [Id, Comment, Message]
                        "refreshAfterSuccess": true,
                        "onError": {
                            "text": "Başarısız."
                        },
                        "onSuccess": {
                            "text": "Başarılı."
                        }
                    },
                    "confirmation": {
                      "message": "Are you sure you want to continue to resend?"
                    },
                    "executeWhen": {
                      // "condition": [ "State", "=", "10" ],
                      "eval": "{State} == 10 && {EnvelopeStatus} == 3"
                    }
                }
            ],
            "subModels": [
              {
                "name": "LicenseModule",
                "title": "License Modules",
                "type": "list",
                "showIn": [
                  "tab"
                ],
                "route": "/module",
                "icon": "view_module",
                "relationFieldNames": [
                  [
                    "Id",
                    "LicenseId"
                  ],
                  [
                    "ProgramId",
                    "ProgramId"
                  ]
                ]
              },
            "columns": [
              {
                "name": "CustomerId",
                "type": "numeric",
                "allowEditing": true,
                "required": true,
                "colSpan": 2,
                "helpText": "Lisansın sahibi olan müşteri.",
                "data": {
                  "type": "customStore",
                  "name": "Customer",
                  "url": "/api/entity?name=Customer",
                  "valueExpr": "Id",
                  "displayExpr": "Name"
                },
                "editWith": {
                  "type": "table",
                  "key": "Id",
                  "valueExpr": "Id",
                  "displayExpr": "Name",
                  "columns": [
                    {
                      "name": "Name",
                      "type": "text"
                    },
                    {
                      "name": "TaxNumber",
                      "type": "text"
                    }
                  ]
                }
              },
              {
                "name": "ProgramId",
                "type": "numeric",
                "allowEditing": true,
                "required": true,
                "data": {
                  "type": "customStore",
                  "name": "Program",
                  "url": "/api/entity?name=Program",
                  "valueExpr": "Id",
                  "displayExpr": "Name"
                }
              },
                ...
     */

            #endregion

            var models = new ScreenModelCollection(_configuration["Config:Database:TablePrefix"]);

            foreach (var model in list) {
                if (model.Type.IsClass) {
                    var props = model.Type.GetProperties();
                    models.TryAdd(model);
                    foreach (var item in props) {
                        if (item.PropertyType == typeof(string)) { }
                        else if (item.PropertyType.IsValueType) {
                            if (item.PropertyType.IsEnum) {
                                models.TryAdd(new ScreenModel(null, item.PropertyType, false).HiddenInSidebar());
                            }
                            else if (item.PropertyType.IsGenericType && item.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                                var argumentType = item.PropertyType.GetGenericArguments().FirstOrDefault();
                                if (argumentType.IsEnum) {
                                    models.TryAdd(new ScreenModel(null, argumentType, false).HiddenInSidebar());
                                }
                            }
                        }
                        else if (item.PropertyType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))) {
                            //var argumentType = item.PropertyType.GetGenericArguments().FirstOrDefault();
                        }
                    }
                }
                else {
                    models.TryAdd(model);
                }
            }

            if (models.Count == 0)
                return;

            /* to appsettings.plugin.screen.json **************/
            var enums = models.Where(_ => _.Type.IsEnum).ToList();
            var classes = new List<ScreenModel>();

            var fileName = "appsettings.plugin.screens.json";
            var fileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (!File.Exists(fileFullName))
                return;

            var json = JObject.Parse(File.ReadAllText(fileFullName));
            foreach (var t in models.Where(_ => _.Type.IsClass)) {
                var screen = json["ScreenConfig"]["Screens"][t.Name];
                if (screen != null) {
                    var visible = screen["visible"];
                    if (visible != null) {
                        if ((bool)(visible as JValue).Value || t.Visible) {
                            classes.Add(t);
                        }
                    }
                    else
                        classes.Add(t);
                }
                else
                    classes.Add(t);
            }

            var jsonWriter = new JTokenWriter();

            //ScreenConfig
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("ScreenConfig");
            {

                jsonWriter.WriteStartObject();
                {
                    // Names
                    jsonWriter.WritePropertyName("Names");
                    {
                        jsonWriter.WriteToken(JArray.FromObject(classes.OrderBy(_ => _.Index).Select(_ => _.Name)).CreateReader());
                    }

                    // AutoCompleteLists
                    jsonWriter.WritePropertyName("Lists");
                    {
                        jsonWriter.WriteStartObject();
                        {
                            foreach (var t in enums) {
                                var names = t.Type.GetFields().Where(_ => _.IsLiteral).Select(_ => new { value = _.Name, id = Convert.ChangeType(_.GetValue(null), typeof(int)) });
                                jsonWriter.WritePropertyName(t.Name);
                                jsonWriter.WriteToken(JArray.FromObject(names).CreateReader());
                            }
                            foreach (var item in extraLists) {
                                jsonWriter.WritePropertyName(item.Key);
                                jsonWriter.WriteToken(JArray.FromObject(item.Value).CreateReader());
                            }
                        }
                        jsonWriter.WriteEndObject();
                    }

                    // Screens
                    jsonWriter.WritePropertyName("Screens");
                    {
                        jsonWriter.WriteStartObject();
                        foreach (var t in classes) {
                            jsonWriter.WritePropertyName(t.Name);
                            jsonWriter.WriteStartObject();
                            {
                                var keyFieldName = "Id";
                                var props = t.Type.GetProperties();
                                foreach (var item in props) {
                                    var attr = item.GetCustomAttribute(typeof(Dapper.Contrib.Extensions.KeyAttribute));
                                    if (attr != null)
                                        keyFieldName = item.Name;
                                }

                                var screen = json["ScreenConfig"]["Screens"][t.Name];
                                var screenExists = screen != null;

                                object getVal(string name, object newVal) {
                                    if (screenExists)
                                        if (screen[name] != null)
                                            return screen[name];
                                    return newVal;
                                }

                                jsonWriter.WritePropertyName("title");
                                jsonWriter.WriteValue(getVal("title", t.Name + " Screen"));

                                jsonWriter.WritePropertyName("visible");
                                var visible = getVal("visible", new JValue(t.Visible)) as JValue;
                                jsonWriter.WriteValue(getVal("visible", (bool)visible.Value));

                                if ((bool)visible.Value) {

                                    jsonWriter.WritePropertyName("route");
                                    jsonWriter.WriteValue(getVal("route", Regex.Replace(t.Name, @"[A-Z]", (m) => $"-{m.Value}").Trim('-').ToLower()));

                                    jsonWriter.WritePropertyName("keyFieldName");
                                    jsonWriter.WriteValue(getVal("keyFieldName", keyFieldName));

                                    jsonWriter.WritePropertyName("icon");
                                    jsonWriter.WriteValue(getVal("icon", t.Icon));

                                    jsonWriter.WritePropertyName("hideInSidebar");
                                    jsonWriter.WriteValue(getVal("hideInSidebar", t.HasInSidebar));

                                    jsonWriter.WritePropertyName("isDefinitionModel");
                                    jsonWriter.WriteValue(getVal("isDefinitionModel", t.IsDefinitionModel));

                                    if (t.Editable) {
                                        var editing = screenExists ? json["ScreenConfig"]["Screens"][t.Name]["editing"] : null;
                                        if (editing == null) {
                                            jsonWriter.WritePropertyName("editing");
                                            jsonWriter.WriteStartObject();
                                            {
                                                jsonWriter.WritePropertyName("enabled");
                                                jsonWriter.WriteValue(true);
                                                jsonWriter.WritePropertyName("mode");
                                                jsonWriter.WriteValue("popup");
                                                jsonWriter.WritePropertyName("allowUpdating");
                                                jsonWriter.WriteValue(true);
                                                jsonWriter.WritePropertyName("allowAdding");
                                                jsonWriter.WriteValue(true);
                                                jsonWriter.WritePropertyName("allowDeleting");
                                                jsonWriter.WriteValue(true);
                                            }
                                            jsonWriter.WriteEndObject();
                                        }

                                        /* "editing": {
                                              "enabled": true,
                                              "mode": "popup",
                                              "allowUpdating": true,
                                              "allowAdding": true,
                                              "allowDeleting": true
                                            },
                                         */
                                    }

                                    #region Columns

                                    var columns = screenExists ? json["ScreenConfig"]["Screens"][t.Name]["columns"] : null;

                                    jsonWriter.WritePropertyName("columns");

                                    jsonWriter.WriteStartArray();

                                    JToken getColumn(string colName) {
                                        if (columns != null) {
                                            if (columns is JArray) {
                                                var objects = (columns as JArray).Children();
                                                var c = objects.FirstOrDefault(_ => _.Value<string>("name") == colName);
                                                return c;
                                            }
                                        }
                                        return null;
                                    }

                                    object getColumnFieldVal(JToken col, string field, object newVal) {
                                        if (col != null)
                                            return col[field] ?? newVal;
                                        return newVal;
                                    }

                                    //var columnIndexes = new List<int>();
                                    //for (int i = 0; i <= props.Length; i++)
                                    //    columnIndexes.Add(i);

                                    foreach (var item in props) {

                                        var jsonIgnorAttr = item.GetCustomAttribute<JsonIgnoreAttribute>();
                                        if (jsonIgnorAttr != null)
                                            continue;

                                        List<string> roles = null;
                                        var restrictedRoleAttr = item.GetCustomAttribute<JsonRestrictedRoleAttribute>();
                                        if (restrictedRoleAttr != null)
                                            roles = restrictedRoleAttr.Roles;

                                        bool isEnum = false;
                                        Type secondType = null;
                                        if (!item.PropertyType.IsPublic)
                                            continue;
                                        var column = getColumn(item.Name);

                                        if (column != null)
                                            continue;

                                        if (item.PropertyType.IsValueType) {
                                            if (item.PropertyType.IsGenericType && item.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                                                var argumentType = item.PropertyType.GetGenericArguments().FirstOrDefault();
                                                secondType = argumentType;
                                                isEnum = secondType.IsEnum;
                                            }
                                        }
                                        else if (item.PropertyType == typeof(string)) {
                                        }
                                        else if (item.PropertyType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable))) {
                                            var argumentType = item.PropertyType.GetGenericArguments().FirstOrDefault();
                                            //TODO: subModel, List<PosModel> Poses
                                            continue;
                                        }
                                        else if (item.PropertyType.IsClass) {
                                            //TODO: subModel, CustomerViewModel
                                            continue;
                                        }

                                        jsonWriter.WriteStartObject();
                                        {
                                            //name
                                            jsonWriter.WritePropertyName("name");
                                            jsonWriter.WriteValue(item.Name);

                                            //{
                                            //    jsonWriter.WritePropertyName("index");
                                            //    var indexColumnValue = (long)(getColumnFieldVal(column, "index", new JValue((long)-1)) as JValue).Value;
                                            //    if (indexColumnValue > -1) {
                                            //        if (columnIndexes.Any(_ => _ == indexColumnValue))
                                            //            columnIndexes.Remove((int)indexColumnValue);
                                            //        else {
                                            //            indexColumnValue = columnIndexes[0];
                                            //            columnIndexes.Remove((int)indexColumnValue);
                                            //        }
                                            //    }
                                            //    else {
                                            //        indexColumnValue = columnIndexes[0];
                                            //        columnIndexes.Remove((int)indexColumnValue);
                                            //    }
                                            //    jsonWriter.WriteValue(indexColumnValue);
                                            //}

                                            if (item.PropertyType == typeof(Boolean) || item.PropertyType == typeof(Boolean?)) {
                                                //type
                                                jsonWriter.WritePropertyName("type");
                                                jsonWriter.WriteValue(getColumnFieldVal(column, "type", "check"));
                                            }
                                            else if (item.PropertyType == typeof(DateTime) || item.PropertyType == typeof(DateTime?)) {
                                                // type: datetime, format: LLL
                                                jsonWriter.WritePropertyName("type");
                                                jsonWriter.WriteValue(getColumnFieldVal(column, "type", "datetime"));
                                                jsonWriter.WritePropertyName("format");
                                                jsonWriter.WriteValue(getColumnFieldVal(column, "format", "LLL"));
                                            }
                                            else if (item.PropertyType.IsEnum || isEnum) {
                                                // autoComplete
                                                if (models.Any(_ => _.Type == item.PropertyType)) {
                                                    jsonWriter.WritePropertyName("autoComplete");
                                                    jsonWriter.WriteValue(getColumnFieldVal(column, "autoComplete", item.PropertyType.Name));
                                                }
                                                jsonWriter.WritePropertyName("type");
                                                jsonWriter.WriteValue(getColumnFieldVal(column, "type", "numeric"));
                                            }
                                            else if (item.PropertyType.IsNumeric() || (secondType != null && secondType.IsNumeric())) {
                                                // type: numeric
                                                jsonWriter.WritePropertyName("type");
                                                jsonWriter.WriteValue(getColumnFieldVal(column, "type", "numeric"));
                                            }
                                            else //if (item.PropertyType == typeof(string)) 
                                            {
                                                // type: string
                                                jsonWriter.WritePropertyName("type");
                                                jsonWriter.WriteValue(getColumnFieldVal(column, "type", "text"));
                                            }

                                            var hidenValue = getColumnFieldVal(column, "hidden", (bool?)null);
                                            if (hidenValue != null) {
                                                jsonWriter.WritePropertyName("hidden");
                                                jsonWriter.WriteValue(hidenValue);
                                            }

                                            var translateValue = getColumnFieldVal(column, "translate", (bool?)null);
                                            if (translateValue != null) {
                                                jsonWriter.WritePropertyName("translate");
                                                jsonWriter.WriteValue(translateValue);
                                            }

                                            var widthValue = getColumnFieldVal(column, "width", (int?)null);
                                            if (widthValue != null) {
                                                jsonWriter.WritePropertyName("width");
                                                jsonWriter.WriteValue(widthValue);
                                            }

                                            var rolesValue = getColumnFieldVal(column, "roles", roles);
                                            if (rolesValue != null) {
                                                jsonWriter.WritePropertyName("roles");
                                                jsonWriter.WriteValue(rolesValue);
                                            }

                                            //if (!string.IsNullOrWhiteSpace(item.AutoCompleteRefFor)) {
                                            //    jsonWriter.WritePropertyName("ref");
                                            //    jsonWriter.WriteValue(item.AutoCompleteRefFor);
                                            //}
                                        }
                                        jsonWriter.WriteEndObject();
                                    }

                                    jsonWriter.WriteEndArray();


                                    #endregion

                                    #region Sub Screens
                                    // TODO: Screen Grid altına detay tab'ları ekleyecek
                                    /*
                                    if (t.SubModels.Count > 0) {
                                        jsonWriter.WriteStartArray();

                                        var subscreen = json["ScreenConfig"]["Screens"][t.Name]["subScreens"];
                                        var subscreenExists = screen != null;

                                        object getSubVal(string name, string newVal) {
                                            if (screenExists)
                                                if (screen[name] != null)
                                                    return screen[name];
                                            return newVal;
                                        }

                                        foreach (var sub in t.SubModels) {
                                            jsonWriter.WriteStartObject();
                                            {
                                                jsonWriter.WritePropertyName("name");
                                                jsonWriter.WriteValue(sub.Name);
                                                jsonWriter.WritePropertyName("title");
                                                jsonWriter.WriteValue(getVal("title", t.Name + " Screen"));
                                            }
                                            jsonWriter.WriteEndObject();
                                        }

                                        jsonWriter.WriteEndArray();
                                    }
                                    */
                                    #endregion

                                    jsonWriter.WritePropertyName("assembly");
                                    jsonWriter.WriteValue(t.Type.FullName);
                                }
                            }
                            jsonWriter.WriteEndObject();  // }
                        }
                        jsonWriter.WriteEndObject();  // }
                    }

                    jsonWriter.WritePropertyName("Queries");
                    {

                        jsonWriter.WriteStartObject();
                        foreach (var t in classes) {

                            var keyFieldName = "Id";
                            var props = t.Type.GetProperties();
                            var editingFieldNames = new List<string>();
                            foreach (var item in props) {
                                var attr = item.GetCustomAttribute(typeof(Dapper.Contrib.Extensions.KeyAttribute));
                                //var attrs = item.GetCustomAttributes();
                                if (attr != null)
                                    keyFieldName = item.Name;

                                if (item.GetCustomAttribute(typeof(Dapper.Contrib.Extensions.ColumnAttribute)) != null
                                    && attr == null)
                                    editingFieldNames.Add(item.Name);
                            }

                            if (editingFieldNames.Count == 0)
                                throw new Exception("Column attributes not fount!");

                            var screen = json["ScreenConfig"]["Queries"][t.Name];

                            jsonWriter.WritePropertyName(t.Name); // Order

                            jsonWriter.WriteStartObject();

                            var selectExists = screen == null ? false : screen["SelectQuery"] != null;
                            if (!selectExists) {
                                jsonWriter.WritePropertyName("SelectQuery");
                                {
                                    jsonWriter.WriteStartObject();
                                    jsonWriter.WritePropertyName("Lines");
                                    jsonWriter.WriteStartArray();
                                    jsonWriter.WriteValue("SELECT *");
                                    jsonWriter.WriteValue($"FROM {t.TableName}");
                                    jsonWriter.WriteEndArray();
                                    jsonWriter.WriteEndObject();
                                }
                            }

                            var selectByIdExists = screen == null ? false : screen["SelectById"] != null;
                            if (!selectByIdExists) {
                                jsonWriter.WritePropertyName("SelectById");
                                {
                                    jsonWriter.WriteStartObject();
                                    jsonWriter.WritePropertyName("Lines");
                                    jsonWriter.WriteStartArray();
                                    jsonWriter.WriteValue($"SELECT * FROM {t.TableName} WHERE {keyFieldName} = @{keyFieldName}");
                                    jsonWriter.WriteEndArray();
                                    jsonWriter.WriteEndObject();
                                }
                            }

                            var insertExists = screen == null ? false : screen["InsertQuery"] != null;
                            if (!insertExists && editingFieldNames.Count > 0) {
                                jsonWriter.WritePropertyName("InsertQuery");
                                {
                                    jsonWriter.WriteStartObject();
                                    jsonWriter.WritePropertyName("Lines");
                                    jsonWriter.WriteStartArray();
                                    jsonWriter.WriteValue($"INSERT INTO {t.TableName} ({{Fields}})");
                                    jsonWriter.WriteValue($"VALUES ({{Parameters}})");
                                    jsonWriter.WriteEndArray();
                                    jsonWriter.WriteEndObject();
                                }
                            }

                            var updateExists = screen == null ? false : screen["UpdateQuery"] != null;
                            if (!updateExists && editingFieldNames.Count > 0) {
                                jsonWriter.WritePropertyName("UpdateQuery");
                                {
                                    jsonWriter.WriteStartObject();
                                    jsonWriter.WritePropertyName("Lines");
                                    jsonWriter.WriteStartArray();
                                    jsonWriter.WriteValue($"UPDATE {t.TableName} ");
                                    jsonWriter.WriteValue($"SET {{Fields}}");
                                    jsonWriter.WriteValue($"WHERE {keyFieldName} = @{keyFieldName}");
                                    jsonWriter.WriteEndArray();
                                    jsonWriter.WriteEndObject();
                                }
                            }

                            var deleteExists = screen == null ? false : screen["DeleteQuery"] != null;
                            if (!deleteExists) {
                                jsonWriter.WritePropertyName("DeleteQuery");
                                {
                                    jsonWriter.WriteStartObject();
                                    jsonWriter.WritePropertyName("Lines");
                                    jsonWriter.WriteStartArray();
                                    jsonWriter.WriteValue($"DELETE FROM {t.TableName} ");
                                    jsonWriter.WriteValue($"WHERE {keyFieldName} = @{keyFieldName}");
                                    jsonWriter.WriteEndArray();
                                    jsonWriter.WriteEndObject();
                                }
                            }

                            jsonWriter.WriteEndObject();

                        }
                        jsonWriter.WriteEndObject();
                    }
                }
                jsonWriter.WriteEndObject();
            }
            jsonWriter.WriteEndObject();

            /*
                Member 	Value	Description
                Concat	0	    Concatenate arrays. (Dizileri birleştir.)
                Union	1	    Union arrays, skipping items that already exist.
                Replace	2	    Replace all array items.
                Merge	3	    Merge array items together, matched by index.               
            */

            var generated = ((JObject)jsonWriter.Token);

            generated.Merge(moduleScreenDefaults, _jsonMergeSettings);

            generated.Merge(json, _jsonMergeSettings);

            string result = generated.ToString();
            File.WriteAllText(fileFullName, result);
#if DEBUG
            if (!string.IsNullOrWhiteSpace(pluginSourcePath)) {
                var devFullName = Path.Combine(pluginSourcePath, fileName);
                File.WriteAllText(devFullName, result);
            }
#endif
        }

        /// <summary>
        /// write to appsettings.locales...json
        /// </summary>
        void AddModelsToTranslation(string _pluginSourcePath, List<ScreenModel> models) {

            var classes = new List<ScreenModel>(models);
            foreach (var lng in new string[] { "tr", "en" }) {
                var fileName = string.IsNullOrWhiteSpace(_pluginSourcePath)
                    ? $"appsettings.locales{(lng == "tr" ? "" : ".en")}.json"
                    : $"appsettings.locales.plugin{(lng == "tr" ? "" : ".en")}.json";

                var fileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                if (!File.Exists(fileFullName))
                    continue;

                var json = JObject.Parse(File.ReadAllText(fileFullName));
                var values = new Dictionary<string, object>();
                foreach (var t in classes) {
                    if (!values.ContainsKey(t.Name))
                        continue;
                    var fields = t.Type.IsEnum
                        ? t.Type.GetFields().Where(_ => !_.IsSpecialName).Select(_ => _.Name).ToDictionary(_ => _, _ => _.ToTitleCase())
                        : t.Type.GetProperties().Select(_ => _.Name).ToDictionary(_ => _, _ => _.ToTitleCase());
                    values.Add(t.Name, fields);
                }

                var translations = JObject.FromObject(new {
                    Translations = new Dictionary<string, object>() {
                        { lng, values }
                    }
                });

                translations.Merge(json, _jsonMergeSettings);

                string result = translations.ToString();

#if DEBUG
                if (!string.IsNullOrWhiteSpace(_pluginSourcePath)) {
                    var devFullName = Path.Combine(_pluginSourcePath, fileName);
                    File.WriteAllText(devFullName, result);
                }
#endif

                File.WriteAllText(fileFullName, result);
            }
        }

        void ConfigMigration(string pluginSourcePath, JObject pluginTemplate, params Type[] autoCompleteListTypes) {
            var template = GetConfigTemplate();
            var fileName = "appsettings.config.json";
            var fileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (!File.Exists(fileFullName))
                return;
            var jsonText = File.ReadAllText(fileFullName);
            jsonText = jsonText.Replace("DiyalogoService", "ELogoService");
            jsonText = jsonText.Replace("GIBService", "TNBService");
            var json = JObject.Parse(jsonText);

            var Config = template.GetConfig();

            var templateObj = JObject.FromObject(
                new {
                    ConnectionStrings = new {
                        Default = "Persist Security Info=True;Data Source=;Initial Catalog=;User ID=sa;Password="
                    },
                    Worker = new {
                        Enabled = false,
                        Assembly = ".dll",
                        Controllers = GetControllers(template)
                    },
                    Modules = new { },
                    MultiLanguage = new { Enabled = false, Languages = new string[] { "tr" }, Default = "tr" },
                    ConfigEditing = new {
                        Enabled = true,
                        Fields = template.GetFieldTemplates(""),
                        AutoCompleteLists = template.AutoCompletes
                            .Add<LogLevelTypes>()
                            .Add<OperatorTypes>()
                            .Add("AlertSeverityTypes", GetAlertSeverityTypes())
                            .Add("Roles", GetRoles())
                            .Add(autoCompleteListTypes)
                            .Generate()
                    },
                    Config
                }
            );

            templateObj.Merge(pluginTemplate, _jsonMergeSettings);
            templateObj.Merge(json, _jsonMergeSettings);

            string result = templateObj.ToString();
            var fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            File.WriteAllText(fullName, result);

#if DEBUG
            if (!string.IsNullOrWhiteSpace(pluginSourcePath)) {
                var devFullName = Path.Combine(pluginSourcePath, fileName);
                File.WriteAllText(devFullName, result);
            }
#endif
            DeletePluginConfigFile(pluginSourcePath);
        }


        Dictionary<string, object> GetControllers(ConfigTemplate configTemplate) {
            var controllers = new Type[] {
                typeof(Controllers.AuthController),
                typeof(Controllers.EntityController),
                typeof(Controllers.HelpController),
                typeof(Controllers.LogController),
                typeof(Controllers.NotificationController),
            };
            return configTemplate.GenerateControllers(controllers);
        }

        Dictionary<string, string> GetAlertSeverityTypes() {
            var list = new Dictionary<string, string>();
            list.Add("Info", "info");
            list.Add("Warn", "warning");
            list.Add("Error", "error");
            list.Add("Success", "success");
            return list;
        }
        Dictionary<string, string> GetRoles() {
            var list = new Dictionary<string, string>();
            list.Add("Admin", "Admin");
            list.Add("Writer", "Writer");
            list.Add("Reader", "Reader");
            list.Add("User", "User");
            return list;
        }

        [Obsolete]
        void PluginConfigMigration(string pluginSourcePath, JObject template) {
            if (template == null)
                return;

            var fileName = "appsettings.plugin.config.json";
            var fileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (!File.Exists(fileFullName))
                return;
            var json = JObject.Parse(File.ReadAllText(fileFullName));

            template.Merge(json, _jsonMergeSettings);

            string result = template.ToString();
            var fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            File.WriteAllText(fullName, result);

#if DEBUG
            if (!string.IsNullOrWhiteSpace(pluginSourcePath)) {
                var devFullName = Path.Combine(pluginSourcePath, fileName);
                File.WriteAllText(devFullName, result);
            }
#endif
        }

        void DeletePluginConfigFile(string pluginSourcePath) {
            var fileName = "appsettings.plugin.config.json";
            var fileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (File.Exists(fileFullName))
                File.Delete(fileFullName);
#if DEBUG
            if (!string.IsNullOrWhiteSpace(pluginSourcePath)) {
                var devFullName = Path.Combine(pluginSourcePath, fileName);
                if (File.Exists(devFullName))
                    File.Delete(devFullName);
            }
#endif
        }
    }
}
