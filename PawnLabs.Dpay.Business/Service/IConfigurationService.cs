using PawnLabs.Dpay.Business.Service.Base;
using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Core.Model;

namespace PawnLabs.Dpay.Business.Service
{
    public interface IConfigurationService : IBaseService<Configuration>
    {
        new Task<ConfigurationModel> Get(Configuration entity);

        Task<ConfigurationModel> GetByType(Configuration entity);

        new Task<List<ConfigurationModel>> GetAll(Configuration entity);

        Task<Guid?> Add(ConfigurationModel model);

        Task<bool> Update(ConfigurationModel model);

        Task<bool> DeleteAll(string email);
    }
}
