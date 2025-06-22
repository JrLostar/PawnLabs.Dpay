using PawnLabs.Dpay.Core.Model;

namespace PawnLabs.Dpay.Core.Helper
{
    public interface IConfigurationHelper
    {
        Entity.Configuration GetEntityFromModel(ConfigurationModel model);
        ConfigurationModel GetModelFromEntity(Entity.Configuration entity);
    }
}