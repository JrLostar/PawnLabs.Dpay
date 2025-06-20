using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PawnLabs.Dpay.Api.Controllers.Base
{
    [Authorize]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected string Email
        {
            get
            {
                return User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value ?? string.Empty;
            }
        }
    }
}
