using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreExamples.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreExamples.Services.UserService
{
    public interface IUserService
    {
        Task<IdentityResult> CreteUserAsync(string email, string password);
        Task<SecurityToken> JwtLogin(string email, string password);

        Task<IEnumerable<UserListItemViewModel>> ListOfUsers();
    }
}