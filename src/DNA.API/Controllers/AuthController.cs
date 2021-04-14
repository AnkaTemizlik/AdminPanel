using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using DNA.API.Infrastructure;
using DNA.API.Services.Communication;
using DNA.Domain.Services;
using DNA.API.Models;
using DNA.Domain.Services.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using DNA.API.Services;
using Newtonsoft.Json;
using DNA.API.Resources.Auth;
using System.Linq;
using AutoMapper;
using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using DNA.API.Resources;
using Microsoft.Extensions.Logging;
using System.Globalization;
using DNA.Domain.Exceptions;
using Hangfire;
using DNA.Domain.Resources;
using DNA.Domain.Utils;
using Newtonsoft.Json.Linq;

namespace DNA.API.Controllers {
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IValuerService _valuerService;
        private readonly IEntityService _entityService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private IConfiguration _configuration;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IWritableOptions _writableConfig;
        public AuthController(IUserService userService, IEmailService emailService, IValuerService valuerService, IEntityService entityService, IMapper mapper, ILogger<AuthController> logger, IConfiguration configuration, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions,
            IWebHostEnvironment hostingEnvironment,
            IWritableOptions writableConfig) {
            _userService = userService;
            _emailService = emailService;
            _valuerService = valuerService;
            _entityService = entityService;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _hostingEnvironment = hostingEnvironment;
            _writableConfig = writableConfig;
        }

        // POST api/auth/login
        [HttpPost("auth/login")]
        [ProducesResponseType(typeof(Response<ApplicationUser>), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<IActionResult> Post([FromBody] CredentialsResource credentials) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                var localUser = await _userService.Authenticate(credentials.UserName, credentials.Password, credentials.Key);

                var key = _configuration["AppId:WEB"];
                if (credentials.Key != key)
                    throw new Alert(AlertCodes.KeyIsNotValid);


                if (localUser == null)
                    throw new Alert(AlertCodes.WrongEmailOrPassword);

                if (!localUser.EmailConfirmed)
                    throw new Alert(AlertCodes.EmailAddressIsNotConfirmed);


                return Ok(new Response(_mapper.Map<ApplicationUser>(localUser)));

            }
            catch (Exception ex) {
                return BadRequest(new Response(ex));
            }
        }

        [HttpPost("auth/register")]
        [ProducesResponseType(typeof(Response<ApplicationUser>), 200)]
        public async Task<IActionResult> Register([FromBody] RegistrationResource resource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (resource.Password != resource.PasswordConfirm)
                return BadRequest(Errors.AddErrorToModelState("passwords_not_matched", "Parolalar uyuşmuyor, insan mıyız?", ModelState));

            if (!Regex.IsMatch(resource.Password, _configuration["Validator:Password"]))
                return BadRequest(Errors.AddErrorToModelState("weak_password", "Şifreniz en az 6 karakter, büyük harf, küçük harf ve rakam içermeli.", ModelState));

            if (!Regex.IsMatch(resource.Email, _configuration["Validator:Email"]))
                return BadRequest(Errors.AddErrorToModelState("email_invalid", "Geçerli bir e-posta adresi girin.", ModelState));

            var isExistingUser = await _userService.FindByEmailAsync(resource.Email);
            if (isExistingUser != null)
                return BadRequest(Errors.AddErrorToModelState("user_already_exists", "Bu e-posta adresi zaten kayıtlı.", ModelState));
#if !DEBUG
            //var isRecaptchaOk = await _userService.IsRecaptchaValidAsync(resource.Recaptcha);
            //if (!isRecaptchaOk)
            //    return BadRequest(Errors.AddErrorToModelState("recaptcha_failure", "Google dedi ki: insan mıyız?", ModelState));
#endif

            var user = new ApplicationUser {
                Email = resource.Email,
                FullName = resource.FullName,
                Role = resource.Key == _configuration["AppId:API"] ? "Writer" : "Reader"
            };

            var userCreated = await _userService.CreateAsync(user, resource.Password, 0);
            if (userCreated == null)
                return BadRequest(Errors.AddErrorToModelState("user_create_failure", "Kullanıcı kaydı oluşturulamadı.", ModelState));

            var body = _emailService.CreateBodyForConfirmEmail(userCreated.EmailConfirmationCode);
            var emailSent = await _emailService.SendAsync(body, "E-posta Doğrulama", resource.Email, userCreated.FullName, null);
            if (!emailSent)
                return BadRequest(Errors.AddErrorToModelState("send_email_failure", "Kullanıcı kaydı oluşturulamadı.", ModelState));

            // TODO: işlem başarılı olamazsa, kullanıcı kaydını sil.

            return Ok(new Response<ApplicationUser>(userCreated));
            //return BadRequest(Errors.AddErrorToModelState("something_sure_happened", "Hiçbir şey olmasa bile, kesin bir şeyler oldu.", ModelState));
        }

        [HttpPost("auth/confirm")]
        [ProducesResponseType(typeof(Response), 200)]
        public async Task<IActionResult> Confirm([FromBody] EmailConfirmationResource resource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                ApplicationUser user = await _userService.ConfirmAsync(resource.Code);
                if (user == null)
                    throw new Exception($"User not found");
                if (!user.EmailConfirmed)
                    throw new Exception($"Error on confirmation");
                return Ok(new Response((object)new { user.PasswordConfirmationCode, user.IsInitialPassword }));
            }
            catch (Exception ex) {
                return Ok(new Response(ex));
            }
        }

        [HttpPost("auth/recovery")]
        [ProducesResponseType(typeof(Response), 200)]
        public async Task<IActionResult> PasswordRecovery([FromBody] PasswordRecoveryResource resource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            string code = await _userService.RecoveryAsync(resource.Email);
            if (code == null)
                return BadRequest(Errors.AddErrorToModelState("password_recovery_failure", "İşlem tamamlanmadı.", ModelState));

            string body = _emailService.CreateBodyForRecoveryPassword(code);
            var emailSent = await _emailService.SendAsync(body, "Şifre Sıfırlama", resource.Email, string.Empty, null);
            if (!emailSent)
                return BadRequest(Errors.AddErrorToModelState("send_email_failure", "İşlem tamamlanmadı.", ModelState));

            return Ok(new Response());
        }

        [HttpPost("auth/changePassword")]
        [ProducesResponseType(typeof(Response), 200)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordResource resource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (resource.Password != resource.PasswordConfirm)
                return BadRequest(Errors.AddErrorToModelState("passwords_not_matched", "Parolalar uyuşmuyor, insan mıyız?", ModelState));

            if (string.IsNullOrWhiteSpace(resource.Code))
                // TODO: Giriş yapmış kullanıcının şifre değiştirebilmesi
                // if (User.Identity.IsAuthenticated) 
                // Code olmadan şifre değiştirme işlemi için IsAuthenticated gerekir. buna göre düzenleme yapılacak
                // veya changePassword sayfası açılırken IsAuthenticated ise, api'ye gelip code alır ve normal ChangePassword çağırılacak. 
                // Code üretmek için recoveryAsync çağrılır ama e-posta göndermemek lazım
                return BadRequest(Errors.AddErrorToModelState("confirmation_code_required", "Onay kodu gerekiyor.", ModelState));

            if (!Regex.IsMatch(resource.Password, _configuration["Validator:Password"]))
                return BadRequest(Errors.AddErrorToModelState("weak_password", "Şifreniz en az 6 karakter, büyük harf, küçük harf ve rakam içermeli.", ModelState));

            if (!Regex.IsMatch(resource.Email, _configuration["Validator:Email"]))
                return BadRequest(Errors.AddErrorToModelState("email_invalid", "Geçerli bir e-posta adresi girin.", ModelState));

            var user = await _userService.FindByEmailAsync(resource.Email);
            if (user == null)
                return BadRequest(Errors.AddErrorToModelState("email_not_failure", "E-posta kayıtlı değil.", ModelState));

            bool ok = await _userService.ChangePasswordAsync(user.Id, resource.Password, resource.Code);
            if (!ok)
                return BadRequest(Errors.AddErrorToModelState("change_password_failure", "Yeni şifre kaydedilemedi.", ModelState));

            return Ok(new Response());
        }

        [HttpGet("auth/settings")]
        [Authorize(Policy = Policies.ReadOnly)]
        [ProducesResponseType(typeof(Response<object>), 200)]
        public async Task<IActionResult> GetSettings() {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {

                var configs = await _writableConfig.Get();
                var screenConfig = await _writableConfig.GetScreenConfig();

                if (screenConfig["Preloading"] != null) {
                    foreach (var item in (screenConfig["Preloading"] as JArray)) {
                        var name = item["name"].ToString();
                        var entityResult = await _entityService.ListAsync(new EntityQuery() { Name = name });
                        screenConfig["Lists"][name] = JArray.FromObject(entityResult.Items);
                    }
                }

                var result = new {
                    configs,
                    screenConfig = new {
                        names = screenConfig["Names"],
                        lists = screenConfig["Lists"],
                        screens = screenConfig["Screens"],
                        cards = screenConfig["Cards"],
                        preloading = screenConfig["Preloading"] ?? JArray.Parse("[]"),
                    }
                };
                return Ok(new Response(result));
            }
            catch (Exception ex) {
                return BadRequest(Errors.AddErrorToModelState(ex, ModelState));
            }
        }

        [HttpGet("locales/{lng}/{ns?}")]
        [ProducesResponseType(typeof(Response<object>), 200)]
        public async Task<IActionResult> GetLocalesAsync(string lng, string ns = "common") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                var locales = await _writableConfig.GetLocalesConfigAsync(lng, ns);
                var response = new Response((object)locales);
                return Ok(response);
            }
            catch (Exception ex) {
                return BadRequest(new Response(ex));
            }
        }

        [HttpPost("auth/settings/{id}")]
        [Authorize(Policy = Policies.WriteOnly)]
        [ProducesResponseType(typeof(Response<object>), 200)]
        public async Task<IActionResult> SaveSettings(int id, [FromBody] Dictionary<string, dynamic> changes) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var result = await _writableConfig.Update(id, changes);
            return Ok(new Response(result));
        }

        //[HttpGet("auth/init")]
        //public async Task<IActionResult> Init(string id) {
        //    if (!ModelState.IsValid) {
        //        return BadRequest(ModelState);
        //    }
        //    try {
        //        List<string> roles = new List<string>();
        //        var identity = HttpContext.User.Identity as ClaimsIdentity;
        //        if (identity != null) {
        //            IEnumerable<Claim> claims = identity.Claims;
        //            var roleClaims = identity.FindAll(ClaimTypes.Role);
        //            foreach (var item in roleClaims) {
        //                roles.Add(item.Value);
        //            }
        //        }

        //        //var menus = await _userService.GetMenusAsync(id, roles, User.Identity.IsAuthenticated);
        //        var ok = await Task.FromResult(true);

        //        return Ok(ok);
        //    }
        //    catch (Exception ex) {
        //        return BadRequest(Errors.AddErrorToModelState("get_menus_failure", ex.Message, ModelState));
        //    }
        //}

        [HttpGet("auth/test")]
        [ProducesResponseType(typeof(Response<dynamic>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TestAsync() {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var date = await _userService.GetDatabaseTime();
            return Ok(new Response<dynamic>(new { date = date.ToString("G"), culture = System.Globalization.CultureInfo.CurrentCulture.Name }));

        }

        [HttpGet("auth/users")]
        [Authorize(Policy = Policies.WriteOnly)]
        [ProducesResponseType(typeof(Response<QueryResult<ApplicationUserResource>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersAsync([FromQuery] UserQueryResource query) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                var userQuery = _mapper.Map<UserQueryResource, UserQuery>(query);
                userQuery.CurrentUserId = Convert.ToInt32(User.Identity.Name);
                var queryResult = await _userService.GetUsersAsync(userQuery);
                var resource = _mapper.Map<QueryResult<ApplicationUserResource>>(queryResult);
                return Ok(new Response(resource));
            }
            catch (Exception ex) {
                var alert = _logger.LogError(AlertCodes.GetUsersError, ex);
                return BadRequest(new Response(alert));
            }
        }

        [HttpGet("auth/users/{id}")]
        [Authorize(Policy = Policies.WriteOnly)]
        [ProducesResponseType(typeof(Response<ApplicationUserResource>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByIdAsync(int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                var userQuery = new UserQuery() {
                    UserId = id,
                    CurrentUserId = Convert.ToInt32(User.Identity.Name)
                };

                var queryResult = await _userService.GetUsersAsync(userQuery);
                var resource = _mapper.Map<QueryResult<ApplicationUserResource>>(queryResult);
                var user = resource.Items.FirstOrDefault();
                if (user == null)
                    throw new Exception("User not found");
                return Ok(new Response(user));
            }
            catch (Exception ex) {
                _logger.LogError(ex, "GetUserByIdAsync error: Identity:{0} User:{0}", User.Identity.Name, id);
                return BadRequest(new Response(ex));
            }
        }

        [HttpPost("auth/users")]
        [Authorize(Policy = Policies.WriteOnly)]
        [ProducesResponseType(typeof(Response<ApplicationUserResource>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveUserByIdAsync([FromBody] SaveUserResource resource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {

                #region Validations
                if (resource.Key != _configuration["AppId:WEB"])
                    throw new Exception("Permision Key not found");

                if (string.IsNullOrWhiteSpace(resource.FullName))
                    throw new ArgumentNullException("FullName");

                if (resource.FullName.Length < 6)
                    throw new Exception("Full Name length is not enough");

                if (string.IsNullOrWhiteSpace(resource.Email))
                    throw new ArgumentNullException("Email");

                if (!Regex.IsMatch(resource.Email, _configuration["Validator:Email"]))
                    throw new Exception("E-mail address is not proper format");

                //if (string.IsNullOrWhiteSpace(resource.Password))
                //    throw new ArgumentNullException("Password");

                //if (string.IsNullOrWhiteSpace(resource.PasswordConfirm))
                //    throw new ArgumentNullException("ConfirmPassword");

                //if (resource.Password != resource.PasswordConfirm)
                //    throw new Exception("Passwords are not same.");

                //if (!Regex.IsMatch(resource.Password, _configuration["Validator:Password"]))
                //    throw new Exception("Password is not proper format");

                #endregion

                if (resource.Id > 0) { // UPDATE 

                    var userQuery = new UserQuery() {
                        UserId = resource.Id,
                        CurrentUserId = Convert.ToInt32(User.Identity.Name)
                    };

                    var appUser = await _userService.GetUserByIdAsync(userQuery);
                    if (appUser == null)
                        throw new Exception($"User not found");

                    if (resource.Email != appUser.Email) {
                        // kendisi ise iptal.
                        if (userQuery.UserId == userQuery.CurrentUserId)
                            throw new Exception("You can not change your e-mail address.");

                        var user = _mapper.Map<ApplicationUser>(resource);
                        user.EmailConfirmationCode = _userService.GetUniqueCode();
                        user.EmailConfirmed = false;
                        user.PasswordConfirmationCode = _userService.GetUniqueCode();
                        user.IsInitialPassword = true;
                        appUser = await _userService.UpdateAsync(user);
                        _valuerService.SetCurrentModel(appUser);

                        // var body = _emailService.CreateBodyForConfirmEmail(appUser.EmailConfirmationCode);
                        // var emailSent = await _emailService.SendAsync(body, "E-posta Doğrulama", appUser.Email, appUser.FullName, null);

                        var emailSent = await _emailService.SendAsync(_configuration.GetSection("Config:Smtp:ConfirmationEmailSettings"), null, null);
                        if (!emailSent)
                            throw new Exception("The user registration has been created but e-mail could not be sent to the user.");
                    }
                    else {
                        var user = _mapper.Map<ApplicationUser>(resource);
                        user.EmailConfirmed = appUser.EmailConfirmed;
                        user.EmailConfirmationCode = appUser.EmailConfirmationCode;
                        user.IsInitialPassword = appUser.IsInitialPassword;
                        appUser = await _userService.UpdateAsync(user);
                    }

                    var resultUser = _mapper.Map<ApplicationUserResource>(appUser);
                    return Ok(new Response(resultUser));
                }
                else { // CREATE
                    var user = _mapper.Map<ApplicationUser>(resource);
                    user.EmailConfirmationCode = _userService.GetUniqueCode();
                    user.EmailConfirmed = false;
                    user.PasswordConfirmationCode = _userService.GetUniqueCode();
                    user.IsInitialPassword = true;

                    var userCreated = await _userService.CreateAsync(user, _userService.GetUniqueCode(), 0);
                    if (userCreated == null)
                        throw new Exception("User registration could not be created.");

                    _valuerService.SetCurrentModel(userCreated);

                    //var body = _emailService.CreateBodyForConfirmEmail(userCreated.EmailConfirmationCode);
                    //var emailSent = await _emailService.SendAsync(body, "E-posta Doğrulama", resource.Email, userCreated.FullName, null);
                    var emailSent = await _emailService.SendAsync(_configuration.GetSection("Config:Smtp:ConfirmationEmailSettings"), null, null);
                    if (!emailSent)
                        throw new Exception("The user registration has been created but e-mail could not be sent to the user");
                    var resultUser = _mapper.Map<ApplicationUserResource>(userCreated);
                    return Ok(new Response(resultUser));
                }

            }
            catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
                return BadRequest(new Response(ex));
            }
        }

        [HttpPost("auth/users/{id}/send-confirmation")]
        [Authorize(Policy = Policies.WriteOnly)]
        [ProducesResponseType(typeof(Response<ApplicationUserResource>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendConfirmationEmailAsync(int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {

                var userQuery = new UserQuery() {
                    UserId = id,
                    CurrentUserId = Convert.ToInt32(User.Identity.Name)
                };

                var user = await _userService.GetUserByIdAsync(userQuery);
                if (user == null)
                    throw new Exception($"User not found");

                if (userQuery.UserId == userQuery.CurrentUserId)
                    throw new Exception("You can not send confirmation e-mail yourself.");

                user.EmailConfirmationCode = _userService.GetUniqueCode();
                user.EmailConfirmed = false;
                user.PasswordConfirmationCode = _userService.GetUniqueCode();
                user.IsInitialPassword = true;

                var appUser = await _userService.UpdateAsync(user);
                _valuerService.SetCurrentModel(appUser);

                var emailSent = await _emailService.SendAsync(_configuration.GetSection("Config:Smtp:ConfirmationEmailSettings"), null, null);
                if (!emailSent)
                    throw new Exception("Kullanıcı e-posta adresi güncellendi ancak kullanıcıya e-posta gönderilmedi.");

                var resultUser = _mapper.Map<ApplicationUserResource>(appUser);
                return Ok(new Response(resultUser));

            }
            catch (Exception ex) {
                _logger.LogError(ex, "SendConfirmationEmailAsync error: Identity:{0}", User.Identity.Name);
                return BadRequest(new Response(ex));
            }
        }


        [HttpPost("auth/email/{group?}")]
        [Authorize(Policy = Policies.WriteOnly)]
        [ProducesResponseType(typeof(Response<ApplicationUserResource>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendTestEmail(string group) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {

                var userQuery = new UserQuery() {
                    UserId = Convert.ToInt32(User.Identity.Name),
                    CurrentUserId = Convert.ToInt32(User.Identity.Name)
                };

                var appUser = await _userService.GetUserByIdAsync(userQuery);
                _valuerService.SetCurrentModel(appUser);

                var emailSent = false;
                if (!string.IsNullOrWhiteSpace(group))
                    emailSent = await _emailService.SendAsync(_configuration.GetSection("Config:Smtp:" + group), null, null);
                else {
                    var body = _emailService.GetHtmlBody(Utils.Lorem(10), Utils.Lorem(60));
                    emailSent = await _emailService.SendAsync(body, Utils.Lorem(7), appUser.Email, appUser.FullName, null);
                }

                if (!emailSent)
                    throw new Exception("Kullanıcı e-posta adresi güncellendi ancak kullanıcıya e-posta gönderilmedi.");

                return Ok(new Response<string>() { Resource = "E-mail Sent." });

            }
            catch (Exception ex) {
                _logger.LogError(ex, "Email send error");
                return BadRequest(new Response(ex));
            }
        }

        //public IActionResult Error(int statusCode) {
        //    if (statusCode == 404) {
        //        var statusFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IStatusCodeReExecuteFeature>();
        //        if (statusFeature != null) {
        //            _logger.LogWarning("Handled 404 for url: {OriginalPath}", statusFeature.OriginalPath);
        //        }
        //        return NotFound();
        //    }
        //    return new ObjectResult(null) { StatusCode = statusCode };
        //}
    }
}

