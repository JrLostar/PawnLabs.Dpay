using PawnLabs.Dpay.Core.Enum;

namespace PawnLabs.Dpay.Api.Model.Response
{
    public class ConfigurationResponseModel
    {
        public EnumConfigurationType Type { get; set; }

        public dynamic Value { get; set; }
    }
}
