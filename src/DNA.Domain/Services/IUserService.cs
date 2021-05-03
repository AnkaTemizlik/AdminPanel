using DNA.API.Models;
using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNA.Domain.Services {
    public interface IUserService {
        Task<ApplicationUser> Authenticate(string userName, string password, string key);
        Task<ApplicationUser> CreateAsync(ApplicationUser userIdentity, string password, int branchId);
        Task<ApplicationUser> UpdateAsync(ApplicationUser userIdentity);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<ApplicationUser> FindByEmailAsync(string emaill);
        Task<bool> CheckPasswordAsync(ApplicationUser userIdentity, string password);
        Task<bool> IsRecaptchaValidAsync(string recaptcha);
        Task<ApplicationUser> ConfirmAsync(string code);
        Task<ApplicationUser> RecoveryAsync(string email);
        Task<System.DateTime> GetDatabaseTime();
        Task<bool> ChangePasswordAsync(int id, string password, string passwordConfirmationCode);
        Task<ApplicationUser> GetUserByIdAsync(UserQuery query);
        Task<QueryResult<ApplicationUser>> GetUsersAsync(UserQuery query);
        string GetUniqueCode();

        //Task<Responses<Menu>> GetMenusAsync(string id, List<string> userRoles, bool isAuthenticated);
    }
}
