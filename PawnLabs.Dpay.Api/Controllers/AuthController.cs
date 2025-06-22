using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PawnLabs.Dpay.Business.Service;
using PawnLabs.Dpay.Core.Configuration;
using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Core.Enum;
using PawnLabs.Dpay.Core.Helper;
using PawnLabs.Dpay.Core.Model;

namespace PawnLabs.Dpay.Api.Controllers
{
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private IMemoryCache _memoryCache;

        private ISecurityHelper _securityHelper;
        private IMailHelper _mailHelper;

        private IConfigurationService _configurationService;

        private ModalConfiguration modalConfiguration;

        public AuthController(IMemoryCache memoryCache, ISecurityHelper securityHelper, IMailHelper mailHelper, 
            IConfigurationService configurationService, IOptions<ModalConfiguration> modalConfiguration) 
        {
            _memoryCache = memoryCache;

            _securityHelper = securityHelper;
            _mailHelper = mailHelper;

            _configurationService = configurationService;

            this.modalConfiguration = modalConfiguration.Value;
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

                #region Configurations

                var configurations = await _configurationService.GetAll(new Configuration() { Email = email });

                if(configurations == null || configurations?.Count < 4)
                {
                    if (configurations != null)
                    {
                        var isSuccess = await _configurationService.DeleteAll(email);

                        if (!isSuccess)
                            return BadRequest();
                    }
                    #region ApiKey

                    string apiKeyData = $"email={email}&guid={Guid.NewGuid().ToString()}";

                    string apiKey = _securityHelper.EncryptString(apiKeyData);

                    #endregion

                    #region Modal

                    var modalConfigurationModel = new ModalConfigurationModel()
                    {
                        Logo = modalConfiguration.Logo,
                        BackgroundColor = modalConfiguration.BackgroundColor,
                        TextColor = modalConfiguration.TextColor,
                        ButtonColor = modalConfiguration.ButtonColor
                    };

                    #endregion

                    var id = await _configurationService.Add(new ConfigurationModel() { Email = email, Type = Core.Enum.EnumConfigurationType.ApiKey, Value = apiKey });

                    if (!id.HasValue)
                        return BadRequest("InsertApiKeyFail");

                    id = await _configurationService.Add(new ConfigurationModel() { Email = email, Type = Core.Enum.EnumConfigurationType.WalletAddress, Value = string.Empty });

                    if (!id.HasValue)
                        return BadRequest("InsertWalletAddressFail");

                    id = await _configurationService.Add(new ConfigurationModel() { Email = email, Type = Core.Enum.EnumConfigurationType.Modal, Value = modalConfigurationModel });

                    if (!id.HasValue)
                        return BadRequest("InsertModalFail");

                    id = await _configurationService.Add(new ConfigurationModel() { Email = email, Type = Core.Enum.EnumConfigurationType.Webhook, Value = string.Empty });

                    if (!id.HasValue)
                        return BadRequest("InsertWebhookFail");
                }

                #endregion

                var token = _securityHelper.GenerateToken(email, EnumApplication.Dashboard);

                return Ok(token);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/auth/token")]
        public async Task<IActionResult> Token(string email, string apiKey)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(apiKey))
                    return BadRequest();

                var apiKeyData = _securityHelper.DecryptString(apiKey);

                if (!apiKeyData.Split("&")[0].Split("=")[1].Equals(email))
                    return Unauthorized();

                var configuration = await _configurationService.GetByType(new Configuration() { Email = email, Type = EnumConfigurationType.ApiKey });

                if (configuration == null)
                    return Unauthorized();

                if (!((string)configuration.Value).Equals(apiKey))
                    return Unauthorized();

                var token = _securityHelper.GenerateToken(email, EnumApplication.Modal);

                return Ok(token);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
