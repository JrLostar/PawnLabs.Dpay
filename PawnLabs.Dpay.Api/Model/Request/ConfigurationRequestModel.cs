using PawnLabs.Dpay.Core.Enum;

namespace PawnLabs.Dpay.Api.Model.Request
{
    public class ConfigurationRequestModel
    {
        public EnumConfigurationType Type { get; set; }

        public string Value { get; set; }
    }
}
