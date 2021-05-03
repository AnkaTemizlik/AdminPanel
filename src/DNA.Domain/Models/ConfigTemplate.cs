using DNA.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DNA.Domain.Models {


    public class ConfigTemplate {
        public ConfigEditingTemplate Editing { get; } = new ConfigEditingTemplate();
        public ConfigEditingAutoCompletesTemplate AutoCompletes { get; } = new ConfigEditingAutoCompletesTemplate();
        public ConfigModulesTemplate Modules { get; } = new ConfigModulesTemplate();
        public ConfigProperty Root { get; private set; }

        Dictionary<string, FieldTemplate> _fields;
        bool configGenerated = false;

        public ConfigProperty Property(bool readOnly = false) {
            var p = new ConfigProperty(this, readOnly);
            Root ??= p;
            return p;
        }

        public ConfigProperty RecurringJobsProperty(bool visible, params string[] jobNames) {
            AutoCompletes.AddCrons();
            return Property()
                .Set("RecurringJobs", visible, Property()
                    .AddTextArray("Roles", true, "Admin", "Writer")
                    .SetActions(jobNames, "Jobs", "engineering", "/hangfire", null, visible, Property()
                        .Add("Enabled", false)
                        .AddSelect("Cron", "Crons", "0 * * * *", true)
                        )
                    );
        }

        public Dictionary<string, object> GenerateControllers(params Type[] controllers) {
            var result = new Dictionary<string, object> {
                { "Names", controllers.Select(_=>_.Name.Replace("Controller", "")).ToArray() }
            };
            foreach (var item in controllers) {
                result.Add(item.Name.Replace("Controller", ""), new {
                    Hidden = false,
                    HiddenActions = new string[] { },
                    Actions = item.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(_ => _.Name).ToArray()
                });
            }
            return result;
        }

        public Dictionary<string, FieldTemplate> GetFieldTemplates(string root) {
            if (!configGenerated)
                throw new Exception("Call 'GetConfig' before.");

            if (Root == null)
                throw new ArgumentNullException("ConfigTemplate.Root");

            _fields = new Dictionary<string, FieldTemplate>();
            LoopNestedProps(root, Root);
            var result = _fields.OrderBy(_ => _.Key).ToDictionary(x => x.Key, x => x.Value);
            return result;
        }

        void LoopNestedProps(string name, ConfigProperty prop) {
            foreach (var t in prop.FieldTemplates) {
                var path = $"{name}:{t.Key}".Trim(':');
                _fields.Add(path, t.Value);
            }
            foreach (KeyValuePair<string, object> p in prop) {
                if (p.Value is ConfigProperty) {
                    var path = $"{name}:{p.Key}".Trim(':');
                    LoopNestedProps(path, p.Value as ConfigProperty);
                }
            }
        }

        public void SetConfigGenerated(bool ok) {
            configGenerated = ok;
        }

        public ConfigProperty GetConfig() {
            configGenerated = true;
            return Property()
                .Set("Smtp", Property()
                    .Add("Address", "smtp.gmail.com")
                    .Add("Port", "587")
                    .Add("FromName", "")
                    .Add("FromAddress", "")
                    .Add("UserName", "")
                    .AddPassword("Password", "")
                    .Add("ReplyTo", "")
                    .Add("EnableSsl", true)
                    .Set("Body", Property())
                    .SetAction("SMTPSettingsTestEmail", "Test SMTP Settings", "send", null, "api/auth/email/SMTPSettingsTestEmail", Property()
                        .Add("Enabled", true)
                        .Add("To", "{ApplicationUser.Email}")
                        .Add("ToName", "{ApplicationUser.FullName}")
                        .AddTextArea("Subject", Utils.Utils.Lorem(7))
                        .Set("Body", Property()
                            .AddTextArea("Title", Utils.Utils.Lorem(5))
                            .AddTextArea("Comment", Utils.Utils.Lorem(30))
                            )
                        )
                    .SetEmail("ConfirmationEmailSettings", Property())
                    .SetEmail("PasswordRecoveryEmailSettings", Property())
                    .SetEmail("ExceptionEmailSettings", Property())
                    )
                .Set("Database", false, Property()
                    .Add("TablePrefix", "DNA_")
                    .Add("LogInsertQuery", "INSERT INTO [dbo].[{TablePrefix}NLOG] (MachineName, Logged, Level, Message, Logger, Callsite, Exception, EntityName, EntityKey) VALUES (@MachineName, @Logged, @Level, @Message, @Logger, @Callsite, @Exception, @EntityName, @EntityKey);")
                    .Add("LogSelectQuery", "SELECT [Id],[MachineName],[Logged],[EntityName],[EntityKey],[Level],[Message],[Logger],[Callsite] FROM [{TablePrefix}NLOG]")
                    )
                .Set("Notification", false, "Notifications", Property()
                    .Add("Enabled", false, true)
                    .Add("RefreshCycleInMinutes", 5, true)
                    )
                .Set("WarningMessage", true, Property()
                    .Add("Enabled", false)
                    .AddSelect("Severity", "AlertSeverityTypes", "warning")
                    .Add("Title", "Warning")
                    .AddTextArea("Message", "You are currently working in a test environment")
                    )
                .Set("Company", false, "Company Informations", Property()
                    .Add("CompanyName", "DNA")
                    .Add("CompanyLogo", "")
                    .Add("ProgramName", "DNA.API")
                    .Add("Description", "Access points to the background workers")
                    .Set("Theme", Property()
                        .Add("Name", "blue")
                        )
                .Set("AuthSettings", false, "Auth Settings", Property()
                    .Add("GoPanelOnStart", false)
                    .Add("AllowPasswordChanging", false)
                    .Add("AllowRegistration", false)
                    .Set("ReCAPTCHA", Property()
                        .Add("Enabled", false)
                        .Add("SiteKey", "")
                        .Add("DataTheme", "dark")
                        )
                    )
                )
            ;
        }
    }

    public class ConfigProperty : Dictionary<string, object> {

        private ConfigTemplate _config;
        private bool _readOnly;
        public Dictionary<string, FieldTemplate> FieldTemplates { get; } = new Dictionary<string, FieldTemplate>();
        public ConfigProperty(ConfigTemplate configTemplate, bool readOnly = false) {
            _readOnly = readOnly;
            this._config = configTemplate;
        }
        public ConfigProperty Set(string name, bool visible, string caption, ConfigProperty value) {
            FieldTemplates.Add(name, new FieldTemplate() { visible = visible, readOnly = value._readOnly, caption = caption });
            base.Add(name, value);
            return this;
        }

        public ConfigProperty Set(string name, bool visible, ConfigProperty value) {
            FieldTemplates.Add(name, new FieldTemplate() { visible = visible, readOnly = value._readOnly });
            base.Add(name, value);
            return this;
        }

        public ConfigProperty SetGuard(Services.Communication.Response licenseValidationResponse) {
            return Set("Guard", _config.Property()
                    .Add("BaseUrl", "http://192.168.34.60:57001", restartRequired: true)
                    .Add("PublicKey", "00000000-0000-0000-0000-000000000000", restartRequired: true)
                    .Set("LicenseStatus", false, _config.Property(readOnly: true)
                        .Add("Success", licenseValidationResponse.Success)
                        .Add("LastTry", DateTime.Now.ToString("o"))
                        .AddTextArea("Message", licenseValidationResponse.Message ?? "")
                        .AddTextArea("Solution", licenseValidationResponse.Success ? "" : "Check logs and settings then try restarting the service.")
                        .Add("ErrorCode", licenseValidationResponse.Code)
                        .AddTextArea("Comment", $"{licenseValidationResponse.Comment}")
                        .AddTextArray("Details", false, (licenseValidationResponse?.Details ?? new List<string>()).ToArray())
                    )
                );
        }

        public ConfigProperty SetGuardModule(string moduleName) {
            return Set("Guard", _config.Property()
                    .Set("Modules", true, _config.Property()
                        .Add(moduleName, "00000000-0000-0000-0000-000000000000", true)
                    )
                );
        }

        public ConfigProperty Set(string name, ConfigProperty value) {
            FieldTemplates.Add(name, new FieldTemplate() { readOnly = value._readOnly });
            base.Add(name, value);
            return this;
        }

        public ConfigProperty SetEmail(string name, ConfigProperty value) {

            /*
            "Enabled": false,
            "To": "mehmet.orakci@dnaproje.com.tr",
            "ToName": "Mehmet Orakci",
            "Subject": "{Program.Name} Ürün lisans süresi doluyor",
            "Body": {
              "Title": "{Program.Name} Ürün lisans süresi doluyor",
              "Comment": "<b>{Customer.Name}</b> {License.StartDate} {License.EndDate}"
            }
        */
            //    "action": {
            //        "text": "Test",
            //  "icon": "send",
            //  "apiUrl": "api/auth/email"
            //}
            value
                .Add("Enabled", false)
                .Add("To", "")
                .Add("ToName", "")
                .AddTextArea("Subject", "")
                .Set("Body", new ConfigProperty(_config)
                    .AddTextArea("Title", "")
                    .AddTextArea("Comment", "")
                    )
                ;

            FieldTemplates.Add(name, new FieldTemplate() { readOnly = value._readOnly });
            base.Add(name, value);
            return this;
        }

        public ConfigProperty SetAction(string name, string text, string icon, string externalUrl, string apiUrl, ConfigProperty value) {
            FieldTemplates.Add(name, _config.Editing.Action(text, icon, externalUrl, apiUrl));
            base.Add(name, value);
            return this;
        }

        public ConfigProperty SetActions(string[] names, string text, string icon, string externalUrl, string apiUrl, bool visible, ConfigProperty value) {
            foreach (var name in names) {
                FieldTemplates.Add(name, _config.Editing.Action(text, icon, externalUrl, null, apiUrl, visible ? null : (bool?)false));
                base.Add(name, value);
            }
            return this;
        }

        public ConfigProperty Add(string name, bool value, bool? restartRequired = null) {
            FieldTemplates.Add(name, _config.Editing.Check(null, restartRequired));
            base.Add(name, value);
            return this;
        }

        public ConfigProperty Add(string name, int value, bool? restartRequired = null) {
            FieldTemplates.Add(name, _config.Editing.Number(null, restartRequired));
            base.Add(name, value);
            return this;
        }

        public ConfigProperty Add(string name, decimal value, bool? restartRequired = null) {
            FieldTemplates.Add(name, _config.Editing.Number(null, restartRequired));
            base.Add(name, value);
            return this;
        }

        public ConfigProperty Add(string name, string value, bool? restartRequired = null) {
            FieldTemplates.Add(name, _config.Editing.Text(null, restartRequired));
            base.Add(name, value);
            return this;
        }

        public ConfigProperty AddPassword(string name, string value, bool? restartRequired = null) {
            FieldTemplates.Add(name, _config.Editing.Pass(null, restartRequired));
            base.Add(name, value);
            return this;
        }

        public ConfigProperty Add(string name, DateTime value, string caption, string format) {
            FieldTemplates.Add(name, _config.Editing.DateTime(caption, format));
            base.Add(name, value);
            return this;
        }

        public ConfigProperty AddSelect(string name, string options, string value, bool? restartRequired = false) {
            FieldTemplates.Add(name, _config.Editing.Select(options, null, restartRequired));
            base.Add(name, value);
            return this;
        }

        public ConfigProperty AddSelect(string name, string options, int value, bool? restartRequired = false) {
            FieldTemplates.Add(name, _config.Editing.Select(options, null, restartRequired));
            base.Add(name, value);
            return this;
        }

        public ConfigProperty AddSelect<T>(string name, T value, bool? restartRequired = false) where T : Enum {
            FieldTemplates.Add(name, _config.Editing.Select(typeof(T).Name, null, restartRequired));
            base.Add(name, value);
            return this;
        }

        public ConfigProperty AddTextArea(string name, string value) {
            FieldTemplates.Add(name, _config.Editing.TextArea());
            base.Add(name, value);
            return this;
        }

        public ConfigProperty AddTextArray(string name, bool? restartRequired = false, params string[] values) {
            FieldTemplates.Add(name, _config.Editing.TextArray(null, restartRequired));
            base.Add(name, values);
            return this;
        }

        public ConfigProperty AddKeyValue(string name, string keyCaption_, string valueCaption_, string caption_, params KeyValue<string>[] defaultKeyValues) {
            FieldTemplates.Add(name, _config.Editing.KeyValue(keyCaption_, valueCaption_, caption_));
            base.Add(name, defaultKeyValues.ToArray());
            return this;
        }

        public ConfigProperty AddKeyProperty(string name, string keyCaption_, string valueCaption_, string caption_, params KeyValue<string, ConfigProperty>[] defaultKeyValues) {
            FieldTemplates.Add(name, _config.Editing.KeyValue(keyCaption_, valueCaption_, caption_));
            base.Add(name, defaultKeyValues.ToArray());
            return this;
        }
    }
    public class ConfigEditingTemplate {
        public FieldTemplate Empty(string caption_ = null) {
            var template = new FieldTemplate().Empty(caption_);
            return template;
        }
        public FieldTemplate Pass(string caption_ = null, bool? restartRequired = null) {
            var template = new FieldTemplate().Pass(caption_, restartRequired);
            return template;
        }
        public FieldTemplate Check(string caption_ = null, bool? restartRequired = null) {
            var template = new FieldTemplate().Check(caption_, restartRequired);
            return template;
        }
        public FieldTemplate Select(string options_, string caption_ = null, bool? restartRequired_ = null) {
            var template = new FieldTemplate().Select(options_, caption_, restartRequired_);
            return template;
        }
        public FieldTemplate AutoComplete(string name, string caption_ = null, bool? restartRequired_ = null) {
            var template = new FieldTemplate().AutoComplete(name, caption_, restartRequired_);
            return template;
        }
        public FieldTemplate AutoComplete(string caption_ = null, bool? restartRequired_ = null, params string[] names) {
            var template = new FieldTemplate().AutoComplete(caption_, restartRequired_, names);
            return template;
        }
        public FieldTemplate KeyValue(string keyCaption_, string valueCaption_, string caption_ = null, bool? restartRequired_ = null) {
            var template = new FieldTemplate().KeyValue(keyCaption_, valueCaption_, caption_, restartRequired_);
            return template;
        }
        public FieldTemplate TextArray(string caption_ = null, bool? restartRequired_ = null) {
            var template = new FieldTemplate().TextArray(caption_, restartRequired_);
            return template;
        }
        public FieldTemplate TextArea(string caption_ = null, bool? restartRequired_ = null) {
            var template = new FieldTemplate().TextArea(caption_, restartRequired_);
            return template;
        }
        public FieldTemplate Text(string caption_ = null, bool? restartRequired_ = null) {
            var template = new FieldTemplate().Text(caption_, restartRequired_);
            return template;
        }
        public FieldTemplate Number(string caption_ = null, bool? restartRequired_ = null) {
            var template = new FieldTemplate().Number(caption_, restartRequired_);
            return template;
        }
        public FieldTemplate DateTime(string caption_ = null, string format_ = null, bool? restartRequired_ = null) {
            var template = new FieldTemplate().DateTime(format_, caption_, restartRequired_);
            return template;
        }
        public FieldTemplate Action(string text = null, string icon = null, string externalUrl = null, string apiUrl = null, string caption = null, bool? visible = null) {
            var template = new FieldTemplate() { visible = visible }.Action(text, icon, externalUrl, apiUrl, caption);
            return template;
        }

    }

    public class ConfigModulesTemplate {

        Dictionary<string, object> _modules = new Dictionary<string, object>();

        public Dictionary<string, object> Generate() {
            var modules = new Dictionary<string, object>();
            modules.Add("Names", _modules.Keys.ToArray());
            foreach (var _ in _modules)
                modules.Add(_.Key, _.Value);
            return modules;
        }

        public ConfigModulesTemplate Add(string name, string assembly) {
            _modules.Add(name, new {
                Enabled = true,
                Assembly = assembly
            });
            return this;
        }

        public ConfigModulesTemplate Add(string name, string assembly, string controllerName = null, bool controllerVisible = false, string[] controllerHiddenActions = null) {
            _modules.Add(name, new {
                Enabled = true,
                Assembly = assembly,
                Controller = new {
                    Name = name,
                    Visible = controllerVisible,
                    HiddenActions = controllerHiddenActions
                }
            });
            return this;
        }
    }

    public class ConfigEditingAutoCompletesTemplate {

        Dictionary<string, object> _autoCompletes = new Dictionary<string, object>();

        public Dictionary<string, object> Generate() {
            if (!_autoCompletes.ContainsKey(typeof(User).Name))
                Add<User>();
            return _autoCompletes;
        }

        public ConfigEditingAutoCompletesTemplate Add(Type[] types) {
            foreach (var item in types)
                Add(item);
            return this;
        }

        public ConfigEditingAutoCompletesTemplate Add<T>(string name, Dictionary<string, T> captionValueList) {
            _autoCompletes
                .Add(name, captionValueList
                    .Select(_ => new {
                        caption = _.Key,
                        value = _.Value
                    })
                );
            return this;
        }

        public ConfigEditingAutoCompletesTemplate Add(Type type) {
            if (!_autoCompletes.ContainsKey(type.Name)) {
                if (type.IsClass) {
                    var names = type.GetProperties().Select(_ => _.Name);
                    _autoCompletes.Add(type.Name, names);
                }
                else if (type.IsEnum) {
                    var names = type.GetFields().Where(_ => _.IsLiteral).Select(_ => _.Name);
                    _autoCompletes
                        .Add(type.Name, type.GetFields()
                            .Where(_ => _.IsLiteral)
                            .Select(_ => new {
                                caption = _.Name,
                                value = _.GetValue(null)
                            })
                            .OrderBy(_ => _.value)
                            .ToList()
                        );
                }
            }
            return this;
        }

        public ConfigEditingAutoCompletesTemplate Add<T>() {
            Add(typeof(T));
            return this;
        }

        internal ConfigEditingAutoCompletesTemplate AddCrons() {

            if (_autoCompletes.ContainsKey("Crons"))
                return this;

            var crons = new Dictionary<string, string>();
            crons.Add("Dakikada 1", "* * * * *");
            crons.Add("Her 10 dakikada 1", "*/10 * * * *");
            crons.Add("Her 15 dakikada 1", "*/15 * * * *");
            crons.Add("Her 30 dakikada 1", "30 * * * *");
            crons.Add("Her saat başında", "0 * * * *");
            crons.Add("Her 6 saatte 1", "0 */6 * * *");
            crons.Add("Günde 1 kez saat 00:00'da", "0 0 * * *");
            crons.Add("Günde 1 kez saat 02:00'da", "0 2 * * *");
            crons.Add("Günde 1 kez saat 04:00'da", "0 4 * * *");
            crons.Add("Günde 1 kez saat 06:00'da", "0 6 * * *");
            _autoCompletes.Add("Crons", crons.Select(_ => new { caption = _.Key, value = _.Value }));
            return this;
        }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class FieldTemplate {

        string _path;

        public bool? visible { get; set; }
        public bool? readOnly { get; set; }

        public string caption { get; set; }

        /// <summary>
        /// password, keyValue, textArea, select (set 'options' value)
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// find from ConfigEditing:AutoCompleteLists. "name" or ["name1", "name2"]
        /// </summary>
        public dynamic autoComplete { get; set; }
        public string keyCaption { get; set; }
        public string valueCaption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string options { get; set; }
        public string format { get; set; }
        public bool? restartRequired { get; set; }
        public FieldActionTemplate action { get; set; }

        public string GetPath() {
            return _path;
        }

        public FieldTemplate Path(string path) {
            _path = path;
            return this;
        }

        public FieldTemplate Text(string caption_ = null, bool? restartRequired_ = null) {
            type = "text";
            caption = caption_;
            restartRequired = restartRequired_;
            return this;
        }

        public FieldTemplate TextArea(string caption_ = null, bool? restartRequired_ = null) {
            type = "textArea";
            caption = caption_;
            restartRequired = restartRequired_;
            return this;
        }

        public FieldTemplate Pass(string caption_ = null, bool? restartRequired_ = null) {
            type = "password";
            caption = caption_;
            restartRequired = restartRequired_;
            return this;
        }
        public FieldTemplate Check(string caption_ = null, bool? restartRequired_ = null) {
            type = "check";
            caption = caption_;
            restartRequired = restartRequired_;
            return this;
        }

        public FieldTemplate Select(string options_, string caption_ = null, bool? restartRequired_ = null) {
            type = "select";
            caption = caption_;
            options = options_;
            restartRequired = restartRequired_;
            return this;
        }

        public FieldTemplate AutoComplete(string name, string caption_ = null, bool? restartRequired_ = null) {
            autoComplete = name;
            type = "textArea";
            caption = caption_;
            restartRequired = restartRequired_;
            return this;
        }

        public FieldTemplate AutoComplete(string caption_ = null, bool? restartRequired_ = null, params string[] names) {
            autoComplete = names.ToArray();
            type = "textArea";
            caption = caption_;
            restartRequired = restartRequired_;
            return this;
        }

        public FieldTemplate KeyValue(string keyCaption_, string valueCaption_, string caption_ = null, bool? restartRequired_ = null) {
            keyCaption = keyCaption_;
            valueCaption = valueCaption_;
            type = "keyValue";
            caption = caption_;
            restartRequired = restartRequired_;
            return this;
        }

        public FieldTemplate TextArray(string caption_ = null, bool? restartRequired_ = null) {
            type = "textArray";
            caption = caption_;
            restartRequired = restartRequired_;
            return this;
        }

        public FieldTemplate DateTime(string format_ = null, string caption_ = null, bool? restartRequired_ = null) {
            type = "date";
            format = format_;
            caption = caption_;
            restartRequired = restartRequired_;
            return this;
        }

        public FieldTemplate Number(string caption_ = null, bool? restartRequired_ = null) {
            type = "number";
            caption = caption_;
            restartRequired = restartRequired_;
            return this;
        }

        public FieldTemplate Action(string caption_ = null, bool? restartRequired_ = null) {
            type = "number";
            caption = caption_;
            restartRequired = restartRequired_;
            return this;
        }

        public FieldTemplate Empty(string caption_) {
            caption = caption_;
            return this;
        }

        public FieldTemplate Action(string text_ = null, string icon_ = null, string externalUrl_ = null, string apiUrl_ = null, string caption_ = null) {
            type = "action";
            caption = caption_;
            action = new FieldActionTemplate {
                text = text_,
                icon = icon_,
                externalUrl = externalUrl_,
                apiUrl = apiUrl_
            };
            return this;
        }
    }

    public class FieldActionTemplate {
        public string text { get; set; }
        public string icon { get; set; }
        public string externalUrl { get; set; }
        public string apiUrl { get; set; }
    }
}
