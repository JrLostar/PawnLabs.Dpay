using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawnLabs.Dpay.Core.Enum;

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

        protected EnumApplication Application
        {
            get
            {
                return (EnumApplication)int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Application")?.Value ?? "0");
            }
        }
    }
}
