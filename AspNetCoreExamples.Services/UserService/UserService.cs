using AspNetCoreExamples.EntityFramework;
using AspNetCoreExamples.Models.Users;
using AspNetCoreExamples.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreExamples.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _settigns;

        public UserService(UserManager<ApplicationUser> userManager, IOptions<AppSettings> options,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _settigns = options.Value;
        }

        public async Task<IdentityResult> CreteUserAsync(string email, string password)
        {
            var account = new ApplicationUser
            {
                Email = email,
                UserName = email
            };

            return await _userManager.CreateAsync(account, password);
        }

        public async Task<SecurityToken> JwtLogin(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_settigns.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Email),
                    new Claim("id",user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(_settigns.TokenLifeTimeInMunutes),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                return tokenHandler.CreateToken(tokenDescriptor);
            }

            return null;
        }

        public async Task<IEnumerable<UserListItemViewModel>> ListOfUsers()
        {
            return await _context.Users
                .Select(x => new UserListItemViewModel()
                {
                    Id = x.Id,
                    Email = x.Email
                })
                .ToListAsync();
        }
    }
}
