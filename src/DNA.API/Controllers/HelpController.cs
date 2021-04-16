using DNA.Domain.Exceptions;
using DNA.Domain.Services.Communication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using DNA.Domain.Services;
using System.IO;
using DNA.Domain.Models.Pages;

namespace DNA.API.Controllers {
    [Route("api/help")]
    [Produces("application/json")]
    [ApiController]
    public class HelpController : ControllerBase {
        ITranslationService _translation;
        public HelpController(ITranslationService translation) {
            _translation = translation;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Response), 200)]
        public async Task<IActionResult> Get() {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                var pagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pages");

                var menus = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Menu>>(System.IO.File.ReadAllText(Path.Combine(pagesPath, "map.json")));
                var helpMenu = menus.Find(_ => _.label == "Help");

                if (helpMenu == null)
                    throw new Exception("Help menu not found");

                await Task.CompletedTask;
                return Ok(new Response(helpMenu));
            }
            catch (Exception ex) {
                var alert = new Alert(AlertCodes.GetHelpDocError, ex);
                return BadRequest(new Response(alert));
            }
        }

        [HttpGet("{path}/{page?}")]
        [ProducesResponseType(typeof(Response), 200)]
        public async Task<IActionResult> GetByName(string path, string page) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                var root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pages", "Help", path, page??"");

                var pageName = $"{root}.{System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName}.html";
                if (!System.IO.File.Exists(pageName))
                    pageName = $"{root}.html";
                await Task.CompletedTask;
                return Ok(new Response(new DNA.Domain.Models.KeyValue<string>("html", System.IO.File.ReadAllText(pageName))));
            }
            catch (Exception ex) {
                var alert = new Alert(AlertCodes.GetHelpDocError, ex);
                return BadRequest(new Response(alert));
            }
        }


        [HttpGet("alert-codes")]
        [ProducesResponseType(typeof(Response), 200)]
        public async Task<IActionResult> GetAlertCodes() {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                // <summary>
                // 1** Validations
                // 2** Startup / Configuration / Auth / Users / EMail Service / Help
                // 3** NoX / E-Adaptor 
                // 4** ELogo / GIB
                // 5** 
                // 6** 
                // 7** 
                // 8** JOBs / Database / SQL Server / Logging
                // 900-920 Entity Service
                // 921-999 Plugin exceptions and warnings
                // </summary>

                List<dynamic> dictionary = new List<dynamic>();
                List<dynamic> data = new List<dynamic>();

                foreach (var item in typeof(AlertCodes).GetTypeInfo().DeclaredMembers) {
                    if (!item.IsCollectible) {
                        var key = item as FieldInfo;
                        if (key != null) {
                            var keyValue = key.GetValue(new AlertCodes()) as Domain.Models.KeyValue<int>;
                            if (keyValue != null)
                                dictionary.Add(new {
                                    //Group = AlertCodes.Groups[keyValue.Key / 100],
                                    keyValue.Key,
                                    keyValue.Value,
                                    Comment = _translation.T(keyValue.Value)
                                });
                        }
                    }
                }

                foreach (var item in AlertCodes.Groups) {
                    data.Add(new {
                        GroupId = item.Key,
                        GroupName = item.Value,
                        Data = dictionary.Where(_ => _.Key / 100 == item.Key).OrderBy(_ => _.Key).ToList()
                    });
                }

                dictionary.Clear();

                //foreach (var item in typeof(GIBAlertCodes).GetTypeInfo().DeclaredMembers) {
                //    if (!item.IsCollectible) {
                //        var key = item as FieldInfo;
                //        if (key != null) {
                //            var keyValue = key.GetValue(new AlertCodes()) as Domain.Models.KeyValue<(int Status, int Code)>;
                //            if (keyValue != null)
                //                dictionary.Add(new {
                //                    //Group = AlertCodes.Groups[keyValue.Key / 100],
                //                    Status = $"{(Domain.Models.Diyalogo.DiyalogoStatus)keyValue.Key.Status}",
                //                    Key = keyValue.Key.Code,
                //                    keyValue.Value,
                //                    Comment = _translation.T(keyValue.Value)
                //                });
                //        }
                //    }
                //}
                //data.Add(new {
                //    GroupId = "10",
                //    GroupName = "Messages returned from the GIB",
                //    Data = dictionary
                //});

                await Task.CompletedTask;
                return Ok(new Response(data));
            }
            catch (Exception ex) {
                var alert = new Alert(AlertCodes.GetHelpDocError, ex);
                return BadRequest(new Response(alert));
            }
        }
    }
}

