using Newtonsoft.Json;
using PawnLabs.Dpay.Core.Enum;
using PawnLabs.Dpay.Core.Model;

namespace PawnLabs.Dpay.Core.Helper.Impl
{
    public class ConfigurationHelper : IConfigurationHelper
    {
        public ConfigurationHelper()
        {
        }

        public Entity.Configuration GetEntityFromModel(ConfigurationModel model)
        {
            if (model == null)
                return null;

            var entity = new Entity.Configuration()
            {
                ID = model.ID,
                Email = model.Email,
                Type = model.Type
            };

            if (model.Type == EnumConfigurationType.Modal)
            {
                entity.Value = JsonConvert.SerializeObject(model.Value);
            }

            return entity;
        }

        public ConfigurationModel GetModelFromEntity(Entity.Configuration entity)
        {
            if (entity == null)
                return null;

            var model = new ConfigurationModel()
            {
                ID = entity.ID,
                Email = entity.Email,
                Type = entity.Type,
                Value = entity.Value
            };

            if (entity.Type == EnumConfigurationType.Modal)
            {
                model.Value = JsonConvert.DeserializeObject<ModalConfigurationModel>(entity.Value) ?? new ModalConfigurationModel();
            }

            return model;
        }
    }
}
