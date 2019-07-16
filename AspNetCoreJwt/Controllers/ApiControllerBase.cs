using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace AspNetCoreExamples.Jwt.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected int? UserId
        {
            get
            {
                var sub = HttpContext.User?.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(sub, out int userId))
                {
                    return userId;
                }

                return null;
            }
        }

        protected string UserName
        {
            get { return HttpContext.User?.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.Name)?.Value; }
        }
    }
}
