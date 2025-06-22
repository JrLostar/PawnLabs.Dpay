using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PawnLabs.Dpay.Api.Controllers.Base;
using PawnLabs.Dpay.Api.Model.Request;
using PawnLabs.Dpay.Api.Model.Response;
using PawnLabs.Dpay.Business.Service;
using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Core.Enum;
using PawnLabs.Dpay.Core.Model;

namespace PawnLabs.Dpay.Api.Controllers
{
    public class ConfigurationController : BaseApiController
    {
        private IConfigurationService _configurationService;

        public ConfigurationController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [HttpGet]
        [Route("/configuration/all")]
        public async Task<IActionResult> GetAll()
        {
            var response = new List<ConfigurationResponseModel>();

            try
            {
                if (string.IsNullOrEmpty(Email))
                    return Unauthorized();

                var configurationModels = await _configurationService.GetAll(new Configuration() { Email = Email });

                if (configurationModels == null || configurationModels?.Count <= 0)
                    return NotFound();

                foreach(var configurationModel in configurationModels)
                {
                    response.Add(new ConfigurationResponseModel()
                    {
                        Type = configurationModel.Type,
                        Value = configurationModel.Value
                    });
                }

                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/configuration")]
        public async Task<IActionResult> Save([FromBody] ConfigurationRequestModel request)
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                    return Unauthorized();

                if (request == null)
                    return BadRequest();

                if (request.Type == EnumConfigurationType.ApiKey)
                    return BadRequest();

                var configurationModel = new ConfigurationModel()
                {
                    Email = Email,
                    Type = request.Type,
                    Value = request.Value
                };

                if(request.Type == EnumConfigurationType.Modal)
                {
                    var modalConfigurationModel = JsonConvert.DeserializeObject<ModalConfigurationModel>(request.Value);

                    if (modalConfigurationModel == null)
                        return BadRequest();

                    configurationModel.Value = modalConfigurationModel;
                }

                var isSuccess = await _configurationService.Update(configurationModel);

                return isSuccess ? Ok() : BadRequest();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
