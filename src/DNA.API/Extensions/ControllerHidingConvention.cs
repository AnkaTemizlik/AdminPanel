using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DNA.API.Extensions {
    public class ControllerHidingConvention : IApplicationModelConvention {
        private readonly IConfiguration _configuration;

        public ControllerHidingConvention(IConfiguration configuration) {
            _configuration = configuration;
        }

        public void Apply(ApplicationModel application) {

            // Plugin Controllers
            var workerSection = _configuration.GetSection("Worker");
            var workerEnabled = workerSection.GetValue<bool>("Enabled");
            if (workerEnabled) {
                ApplyBySection(application, workerSection.GetSection("Controllers"));
            }

            // External Modules Controllers
            {
                var modules = _configuration.GetSection("Modules");
                var names = modules.GetSection("Names").Get<List<string>>();
                if (names != null) {
                    foreach (var name in names) {
                        var moduleSection = modules.GetSection(name);
                        ApplyBySection(application, moduleSection.GetSection("Controllers"));
                    }
                }
            }

            // Internal Modules Controllers
            //foreach (ControllerModel controller in application.Controllers) {
            //    if (controller.ControllerName == "Notification") {
            //        var enabled = _configuration.GetSection("Config:Notification").GetValue<bool>("Enabled");
            //        foreach (ActionModel action in controller.Actions) {
            //            action.ApiExplorer.IsVisible = enabled;
            //        }
            //    }
            //}
        }

        void ApplyBySection(ApplicationModel application, IConfigurationSection controllersSection) {
            var names = controllersSection.GetSection("Names").Get<List<string>>();
            if (names != null) {
                foreach (var name in names) {
                    if (string.IsNullOrWhiteSpace(name))
                        continue;
                    var controller = application.Controllers.FirstOrDefault(_ => _.ControllerName == name);
                    if (controller == null)
                        continue;
                    var controllerSection = controllersSection.GetSection(name);
                    var hiddenActions = controllerSection.GetSection("HiddenActions").Get<List<string>>() ?? new List<string>();
                    var visible = !controllerSection.GetValue<bool>("Hidden");
                    foreach (ActionModel action in controller.Actions) {
                        action.ApiExplorer.IsVisible = visible ? !hiddenActions.Contains(action.ActionName) : false;
                    }
                }
            }
        }
    }
}
