using DNA.API.Models;
using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNA.Domain.Repositories {
    public interface IUserRepository {
        Task<ApplicationUser> FindByIdAsync(int id);
        Task<ApplicationUser> FindByUserNameAndPasswordAsync(string userName, string password);
        Task<ApplicationUser> CreateAsync(ApplicationUser userIdentity, string password, int branchId);
        Task<ApplicationUser> UpdateAsync(ApplicationUser userIdentity);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<ApplicationUser> FindByEmailAsync(string emaill);
        Task<bool> CheckPasswordAsync(ApplicationUser userIdentity, string password);
        Task<ApplicationUser> ConfirmEmailAsync(string emailConfirmationCode);
        Task<ApplicationUser> RecoveryPasswordAsync(string email, string code);
        Task<bool> ChangePasswordAsync(int id, string password, string passwordConfirmationCode);
        Task<DateTime> GetDatabaseTime();
        Task<QueryResult<ApplicationUser>> GetUsersAsync(UserQuery query);

        //Task<IEnumerable<Menu>> GetMenusAsync(string id);
    }
}
