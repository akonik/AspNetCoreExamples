using AspNetCoreExamples.Services.UserService;
using AspNetCoreExamples.ViewModels.Account;
using AspNetCoreExamples.ViewModels.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AspNetCoreExamples.Jwt.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ApiControllerBase
    {
        private IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService ?? throw new System.ArgumentNullException(nameof(userService));
        }

        [HttpPost("create")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateAccount([FromBody]CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var result = await _userService.CreteUserAsync(model.Email, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            var token = await _userService.JwtLogin(model.Email, model.Password);

            if (token != null)
            {
                return new JsonResult(new TokenResponseViewModel
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiresIn = token.ValidTo
                });
            }

            return BadRequest();
        }

        [HttpPost("token")]
        [Produces("application/json")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            var token = await _userService.JwtLogin(model.Email, model.Password);

            if (token != null)
            {
                return new JsonResult(new TokenResponseViewModel
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiresIn = token.ValidTo
                });
            }

            return BadRequest("Invalid credentials.");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("users-list")]
        [Produces("application/json")]
        public async Task<IActionResult> List()
        {
            return Ok(await _userService.ListOfUsers());
        }
        
    }
}
