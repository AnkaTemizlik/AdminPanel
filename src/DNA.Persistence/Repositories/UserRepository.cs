using DNA.API.Models;
using System;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNA.Persistence.Contexts;
using DNA.Domain;
using DNA.Domain.Repositories;
using DNA.Domain.Models.Queries;
using DNA.Domain.Models;
using DNA.Domain.Extentions;

namespace DNA.Persistence.Repositories {

    [Service(typeof(IUserRepository), Lifetime.Scoped)]
    public class UserRepository : BaseRepository, IUserRepository {

        const string _fields = @"[Id]
            , [FullName]
            , [UserName]
            , [Role]
            , [Email]
            , [PhoneNumber]
            , [EmailConfirmed]
            , [EmailConfirmationCode]
            , [PasswordConfirmationCode]
            , [Token]
            , [IsInitialPassword]
            , [LockoutEnd]
            , [LockoutEnabled]
            , [Location] ";

        public UserRepository(IAppDbContext context) : base(context) {

        }

        public async Task<ApplicationUser> FindByIdAsync(int id) {
            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix($@"SELECT {_fields} FROM [{{TablePrefix}}USER] WHERE Id = @Id");

            return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(sql,
                new {
                    Id = id
                });
        }

        public async Task<ApplicationUser> FindByUserNameAndPasswordAsync(string userName, string password) {
            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix($@"SELECT {_fields}
	                FROM [{{TablePrefix}}USER]
	                WHERE Email = @UserName
		                AND [Password] = CONVERT(VARCHAR(1500), HASHBYTES('SHA2_512', CAST(@Password AS VARCHAR(50))), 1)");
            return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(sql,
                new {
                    UserName = userName,
                    Password = password
                });
        }

        public async Task<ApplicationUser> CreateAsync(ApplicationUser userIdentity, string password, int branchId) {

            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix($@"
                    INSERT INTO [{{TablePrefix}}USER] ([FullName], [UserName], [Role], [Email], [PhoneNumber], [EmailConfirmed], [EmailConfirmationCode],[Password], [Token], [Location], [IsInitialPassword], [PasswordConfirmationCode])
                    VALUES (ISNULL(@FullName, ''), ISNULL(@UserName, @Email), @Role, @Email, @PhoneNumber, 0, @EmailConfirmationCode, 
                        CONVERT(VARCHAR(1500), HASHBYTES('SHA2_512', CAST(@Password AS VARCHAR(50))), 1), @Token, @Location, @IsInitialPassword, @EmailConfirmationCode)

                    DECLARE @Id INT = ISNULL(SCOPE_IDENTITY(), 0)

                    SELECT {_fields} 
                    FROM [{{TablePrefix}}USER] 
                    WHERE [Id] = @Id
                ");
            return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(sql,
                new {
                    userIdentity.FullName,
                    UserName = userIdentity.Email,
                    userIdentity.Email,
                    userIdentity.PhoneNumber,
                    userIdentity.EmailConfirmationCode,
                    Password = password,
                    userIdentity.Token,
                    userIdentity.Location,
                    Role = !string.IsNullOrWhiteSpace(userIdentity.Role) ? userIdentity.Role : (branchId > 0 ? "Writer" : "Reader"),
                    BranchId = branchId,
                    userIdentity.IsInitialPassword
                });
        }

        public async Task<ApplicationUser> UpdateAsync(ApplicationUser userIdentity) {

            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix($@"

                    UPDATE u
                    SET [FullName] = ISNULL(@FullName, [FullName]), 
                        [UserName] = @Email, 
                        [Email] = @Email, 
                        [PhoneNumber] = NULL, 
                        [EmailConfirmed] = @EmailConfirmed,
                        [EmailConfirmationCode] = ISNULL(@EmailConfirmationCode, [EmailConfirmationCode]),
                        [PasswordConfirmationCode] = ISNULL(@PasswordConfirmationCode, [PasswordConfirmationCode]),
                        [Role] = ISNULL(@Role, [Role]),
                        [LockoutEnabled] = @LockoutEnabled,
                        [LockoutEnd] = ISNULL(@LockoutEnd, [LockoutEnd])
                    FROM [{{TablePrefix}}USER] AS u
                    WHERE [Id] = @Id

                    SELECT {_fields} 
                    FROM [{{TablePrefix}}USER] 
                    WHERE [Id] = @Id
                ");
            return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(sql,
                new {
                    userIdentity.Id,
                    userIdentity.FullName,
                    userIdentity.Email,
                    userIdentity.PhoneNumber,
                    userIdentity.EmailConfirmed,
                    userIdentity.EmailConfirmationCode,
                    userIdentity.PasswordConfirmationCode,
                    Role = !string.IsNullOrWhiteSpace(userIdentity.Role) ? userIdentity.Role : "Reader",
                    userIdentity.LockoutEnabled,
                    userIdentity.LockoutEnd
                });
        }


        public async Task<ApplicationUser> FindByNameAsync(string userName) {
            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix(@$"
	                SELECT {_fields}
	                FROM [{{TablePrefix}}USER]
	                WHERE [UserName] = @UserName
                ");
            return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(sql,
                new {
                    UserName = userName
                });
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email) {
            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix(@$"
	                SELECT {_fields}
	                FROM [{{TablePrefix}}USER]
	                WHERE [Email] = @Email
                ");
            return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(sql,
                new {
                    Email = email
                });
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser userIdentity, string password) {

            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix($@"
                    DECLARE @Exists BIT = 0
                    IF EXISTS(
	                    SELECT [Id]
	                    FROM [{{TablePrefix}}USER]
	                    WHERE Email = @Email
		                    AND [Password] = CONVERT(VARCHAR(1500), HASHBYTES('SHA2_512', CAST(@Password AS VARCHAR(50))), 1)
	                    )
                    BEGIN
	                    SET @Exists = 1
                    END
                    SELECT @Exists
                ");
            var ok = await connection.QueryFirstAsync<bool>(sql,
                new {
                    userIdentity.Email,
                    Password = password
                });

            return ok;
        }

        public async Task<ApplicationUser> ConfirmEmailAsync(string emailConfirmationCode) {
            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix($@"
                    DECLARE @Id INT = NULL;

	                SELECT @Id = [Id] FROM [{{TablePrefix}}USER] 
                    WHERE EmailConfirmationCode = @EmailConfirmationCode;

                    IF @Id IS NULL 
                        SELECT 0 AS EmailConfirmed FROM [{{TablePrefix}}USER] WHERE Id = @Id 
                    ELSE 
                    BEGIN
                        UPDATE u 
                        SET EmailConfirmed = 1, IsInitialPassword = 1
                        FROM [{{TablePrefix}}USER] u
                        WHERE Id = @Id

                        SELECT {_fields} FROM [{{TablePrefix}}USER] WHERE Id = @Id 
                    END
                ");
            return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(sql,
                new {
                    EmailConfirmationCode = emailConfirmationCode
                });
        }

        public async Task<bool> RecoveryPasswordAsync(string email, string code) {
            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix($@"
                    DECLARE @Id INT = NULL;

                    SELECT @Id = [Id] FROM [{{TablePrefix}}USER] 
                    WHERE Email = @Email;

                    IF @Id IS NULL 
                        SELECT CAST(0 AS BIT)
                    ELSE 
                    BEGIN
                        UPDATE u 
                        SET PasswordConfirmationCode = @PasswordConfirmationCode 
                        FROM [{{TablePrefix}}USER] u
                        WHERE Id = @Id;

                        SELECT CAST(1 AS BIT)

                    END
                ");
            return await connection.QueryFirstOrDefaultAsync<bool>(sql,
                new {
                    Email = email,
                    PasswordConfirmationCode = code
                });
        }

        public async Task<bool> ChangePasswordAsync(int id, string password, string passwordConfirmationCode) {
            using var connection = Context.Connection;
            var sql = Context.SetTablePrefix($@"
                    DECLARE @Exists INT = NULL;

	                SELECT @Exists = [Id] FROM [{{TablePrefix}}USER] 
                    WHERE Id = @Id AND PasswordConfirmationCode = @PasswordConfirmationCode;

                    IF @Exists IS NULL 
                        SELECT CAST(0 AS BIT)
                    ELSE
                    BEGIN
                        UPDATE u 
                        SET IsInitialPassword = 0
                            , Password = CONVERT(VARCHAR(1500), HASHBYTES('SHA2_512', CAST(@Password AS VARCHAR(50))), 1)
                        FROM [{{TablePrefix}}USER] u
                        WHERE Id = @Id;

                        SELECT CAST(1 AS BIT)
                    END
                ");
            return await connection.QueryFirstOrDefaultAsync<bool>(sql,
                new {
                    Id = id,
                    Password = password,
                    PasswordConfirmationCode = passwordConfirmationCode
                });
        }

        public async Task<DateTime> GetDatabaseTime() {
            using var connection = Context.Connection;
            return await connection.QueryFirstOrDefaultAsync<DateTime>(@"SELECT GETDATE()");
        }

        public async Task<QueryResult<ApplicationUser>> GetUsersAsync(UserQuery query) {
            using var connection = Context.Connection;

            query.Take = query.Take <= 0 ? 100 : query.Take;

            var sql = Context.SetTablePrefix(@$"
                DECLARE @CurrentUserRole NVARCHAR(50) 

                IF @UserId <= 0
                    SET @UserId = NULL

                SELECT @CurrentUserRole = [Role]
                FROM [dbo].[{{TablePrefix}}USER] AS u
                WHERE u.Id = @CurrentUserId

                DECLARE @Table TABLE ([Role] NVARCHAR(50))

                IF (@CurrentUserRole = 'Writer')
                BEGIN
	                INSERT @Table SELECT 'Writer'
	                INSERT @Table SELECT 'Reader'
                END

                IF (@CurrentUserRole = 'Admin')
                BEGIN
	                INSERT @Table SELECT 'Admin'
	                INSERT @Table SELECT 'Writer'
	                INSERT @Table SELECT 'Reader'
                END

                SELECT u.*
                FROM [dbo].[{{TablePrefix}}USER] AS u
                INNER JOIN @Table t ON u.[Role] = t.[Role]
                WHERE u.Id = ISNULL(@UserId, u.Id)
                ORDER BY u.Id DESC
                OFFSET ((@Page) * @Take) ROWS FETCH NEXT @Take ROWS ONLY;

                SELECT COUNT(u.Id)
                FROM [dbo].[{{TablePrefix}}USER] AS u
                INNER JOIN @Table t ON u.[Role] = t.[Role]
                WHERE u.Id = ISNULL(@UserId, u.Id);

            ");

            query.Prepare();
            
            var results = await connection.QueryMultipleAsync(sql, (object)query.SqlParameters);

            var users = results.Read<ApplicationUser>().ToList();
            var totalItems = results.ReadSingle<int>();

            return new QueryResult<ApplicationUser> {
                Items = users,
                TotalItems = totalItems
            };
        }

        /*
public async Task<IEnumerable<Menu>> GetMenusAsync(string id) {
  using var connection = Context.Connection;
  return await connection.QueryAsync<Menu>(@"
      WITH m AS (

          SELECT *
          FROM [Menu]
          WHERE AppId = @AppId

          UNION ALL

          SELECT e.*
          FROM [Menu] e
          INNER JOIN m ON m.Id = e.MenuId
      )
      SELECT * FROM m                    
      ",
      new {
          AppId = id
      });
}
*/
    }
}
