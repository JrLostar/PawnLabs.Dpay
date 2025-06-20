using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PawnLabs.Dpay.Core.Helper;

namespace PawnLabs.Dpay.Api.Controllers
{
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private IMemoryCache _memoryCache;

        private ISecurityHelper _securityHelper;
        private IMailHelper _mailHelper;

        public AuthController(IMemoryCache memoryCache, ISecurityHelper securityHelper, IMailHelper mailHelper) 
        {
            _memoryCache = memoryCache;

            _securityHelper = securityHelper;
            _mailHelper = mailHelper;
        }

        [HttpPost]
        [Route("/auth")]
        public async Task<IActionResult> Login(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return BadRequest();

                var verificationCode = await _mailHelper.SendVerificationEmail(email);

                _memoryCache.Set($"VerificationCode-{email}", verificationCode, TimeSpan.FromMinutes(5));

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/auth/verify")]
        public async Task<IActionResult> Verify(string email, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(email))
                    return BadRequest();

                var verificationCode = _memoryCache.Get<string?>($"VerificationCode-{email}");
                
                if(string.IsNullOrEmpty(verificationCode) || !code.Equals(verificationCode))
                    return Unauthorized();

                _memoryCache.Remove($"VerificationCode-{email}");

                var token = _securityHelper.GenerateToken(email);

                return Ok(token);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
