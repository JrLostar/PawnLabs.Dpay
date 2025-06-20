using PawnLabs.Dpay.Core.Enum;
using System.Text.Json.Serialization;

namespace PawnLabs.Dpay.Core.Model
{
    public class ConfigurationModel
    {
        public Guid ID { get; set; }

        public string Email { get; set; }

        public EnumConfigurationType Type { get; set; }

        public dynamic Value { get; set; }
    }

    public class ModalConfigurationModel
    {
        [JsonPropertyName("Logo")]
        public string Logo { get; set; }

        [JsonPropertyName("BackgroundColor")]
        public string BackgroundColor { get; set; }

        [JsonPropertyName("TextColor")]
        public string TextColor { get; set; }

        [JsonPropertyName("ButtonColor")]
        public string ButtonColor { get; set; }
    }
}
