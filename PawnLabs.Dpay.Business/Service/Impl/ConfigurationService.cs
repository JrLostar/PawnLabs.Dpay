using PawnLabs.Dpay.Business.Service.Base.Impl;
using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Core.Helper;
using PawnLabs.Dpay.Core.Model;
using PawnLabs.Dpay.Data.Repository;

namespace PawnLabs.Dpay.Business.Service.Impl
{
    public class ConfigurationService : BaseService<Configuration>, IConfigurationService
    {
        private IConfigurationRepository _configurationRepository;

        private IConfigurationHelper _configurationHelper;

        public ConfigurationService(IServiceProvider serviceProvider, IConfigurationRepository configurationRepository, IConfigurationHelper configurationHelper) : base(serviceProvider)
        {
            _configurationRepository = configurationRepository;

            _configurationHelper = configurationHelper;
        }

        public new async Task<ConfigurationModel> Get(Configuration entity)
        {
            var configuration = await _configurationRepository.Get(entity);

            return _configurationHelper.GetModelFromEntity(configuration);
        }

        public async Task<ConfigurationModel> GetByType(Configuration entity)
        {
            var configurations = await _configurationRepository.GetAll(entity);

            var configuration = configurations?.FirstOrDefault(f => f.Type == entity.Type);

            return _configurationHelper.GetModelFromEntity(configuration);
        }

        public new async Task<List<ConfigurationModel>> GetAll(Configuration entity)
        {
            var configurationModels = new List<ConfigurationModel>();

            var configurations = await _configurationRepository.GetAll(entity);

            foreach (var configuration in configurations)
                configurationModels.Add(_configurationHelper.GetModelFromEntity(configuration));

            return configurationModels;
        }

        public async Task<Guid?> Add(ConfigurationModel model)
        {
            var entity = _configurationHelper.GetEntityFromModel(model);

            return await _configurationRepository.Add(entity);
        }

        public async Task<bool> Update(ConfigurationModel model)
        {
            var entity = _configurationHelper.GetEntityFromModel(model);

            return await _configurationRepository.UpdateByEmail(entity);
        }

        public async Task<bool> DeleteAll(string email)
        {
            return await _configurationRepository.DeleteAll(email);
        }
    }
}
