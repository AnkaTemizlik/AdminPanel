using DNA.API.Infrastructure;
using DNA.API.Models;
using DNA.API.Services.Communication;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using DNA.Domain.Repositories;
using DNA.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(IUserService), Lifetime.Scoped)]
    public class UserService : IUserService {

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        public UserService(IUserRepository userRepository, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions, IConfiguration configuration) {
            _configuration = configuration;
            _userRepository = userRepository;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<ApplicationUser> Authenticate(string username, string password, string key) {

            var localUser = await FindByEmailAsync(username);
            if (localUser == null) {
                throw new Exception("Wrong e-mail or password");
            }
            if(localUser.IsDeleted)
                throw new Exception("Your account has been deleted. Contact the administrator.");

            if (string.IsNullOrWhiteSpace(localUser.PasswordConfirmationCode)) {
                localUser.PasswordConfirmationCode = GetUniqueCode();
                await UpdateAsync(localUser);
            }
            if (string.IsNullOrWhiteSpace(localUser.PasswordConfirmationCode)) {
                localUser.PasswordConfirmationCode = GetUniqueCode();
                await UpdateAsync(localUser);
            }
            if (localUser != null)
                if (await CheckPasswordAsync(localUser, password)) {
                    localUser.Token = await _jwtFactory.GenerateEncodedToken(localUser);
                    return localUser;
                }
            return null;
        }

        private readonly object _lock = new object();
        public async Task<ApplicationUser> CreateAsync(ApplicationUser userIdentity, string password, int branchId) {
            userIdentity.EmailConfirmationCode = GetUniqueCode();
            return await _userRepository.CreateAsync(userIdentity, password, branchId);
        }

        public string GetUniqueCode() {
            lock (_lock) {
                System.Threading.Thread.Sleep(10);
                return Convert.ToInt64(string.Format("{0:yyMMddHHmmssfff}", DateTime.Now)).ToString("X");
            }
        }

        public async Task<ApplicationUser> UpdateAsync(ApplicationUser userIdentity) {
            return await _userRepository.UpdateAsync(userIdentity);
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email) {
            return await _userRepository.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName) {
            return await _userRepository.FindByNameAsync(userName);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser userIdentity, string password) {
            return await _userRepository.CheckPasswordAsync(userIdentity, password);
        }

        public async Task<bool> IsRecaptchaValidAsync(string recaptcha) {
            var result = false;
            var captchaResponse = recaptcha;
            var secretKey = _configuration["Recaptcha:SecretKey"];
            var apiUrl = "https://www.google.com/recaptcha/api/siteverify";

            var request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "POST";

            var postData = string.Format("secret={0}&response={1}", secretKey, captchaResponse);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            using (WebResponse response = request.GetResponse()) {
                var statusDescription = ((HttpWebResponse)response).StatusDescription;

                using (StreamReader stream = new StreamReader(response.GetResponseStream())) {
                    string text = await stream.ReadToEndAsync();
                    JObject jResponse = JObject.Parse(text);
                    result = jResponse.Value<bool>("success");
                }
            }
            return result;
        }

        public async Task<ApplicationUser> ConfirmAsync(string code) {
            var user = await _userRepository.ConfirmEmailAsync(code);
            return user;
        }

        public async Task<ApplicationUser> RecoveryAsync(string email) {
            var code = GetUniqueCode();
            var user = await _userRepository.RecoveryPasswordAsync(email, code);
            return user;
        }

        public async Task<bool> ChangePasswordAsync(int id, string password, string passwordConfirmationCode) {
            return await _userRepository.ChangePasswordAsync(id, password, passwordConfirmationCode);
        }

        public async Task<DateTime> GetDatabaseTime() {
            return await _userRepository.GetDatabaseTime();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(UserQuery query) {
            var queryResult = await GetUsersAsync(query);
            return queryResult.Items.FirstOrDefault();
        }
        public async Task<QueryResult<ApplicationUser>> GetUsersAsync(UserQuery query) {
            var list = await _userRepository.GetUsersAsync(query);
            return list;
        }

        /*
public async Task<Responses<Menu>> GetMenusAsync(string id, List<string> userRoles, bool isAuthenticated) {
var menus = await _userRepository.GetMenusAsync(id);

var rootMenus = menus.Where(_ => _.MenuId == 0 && CheckPerm(_, isAuthenticated, userRoles)).ToList();
foreach (var item in rootMenus) {
if (item.Name == "Contact") {
  item.AdditionalInfo = new {
      Name = "CompanyInfo",
      companyName = " ",
      companyWebsite = "https://xxx.com",
      companyLogo = "https://xxx.com/wp-content/uploads/2019/03/logo.mobile.x75.png",
      address = new string[] {
          "",
          "",
          "İstanbul"
      }
  };
}
AddSubMenus(menus, item, userRoles, isAuthenticated);
}

return new Responses<Menu>(rootMenus, rootMenus.Count);
}

void AddSubMenus(IEnumerable<Menu> menus, Menu menu, List<string> userRoles, bool isAuthenticated) {

if (!menu.AreMenusVisible.GetValueOrDefault(true))
return;

foreach (var m in menus.Where(_ => _.MenuId == menu.Id).OrderBy(_ => _.SortOrder)) {
bool add = CheckPerm(m, isAuthenticated, userRoles);
if (add) {
  menu.Menus.Add(m);
  AddSubMenus(menus, m, userRoles, isAuthenticated);
}
}
}

private bool CheckPerm(Menu m, bool isAuthenticated, List<string> userRoles) {
bool add = false;
if (!m.IsHeaderVisible.GetValueOrDefault(true)) {
add = false;
}
else if (isAuthenticated) {
if (m.Authorize == null)
  add = true;
else if (m.Authorize == "Guest")
  add = false;
else {
  var perm = userRoles.Intersect(m.Authorize.Split(','));
  if (perm.Count() > 0)
      add = true;
}
}
else { // giriş yapmadı
if (m.Authorize == "Guest" || m.Authorize == null)
  add = true;
}

return add;
}*/
    }
}
