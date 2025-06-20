using PawnLabs.Dpay.Core.Entity.Base;
using PawnLabs.Dpay.Core.Enum;

namespace PawnLabs.Dpay.Core.Entity
{
    public class Configuration : IEntity
    {
        public Guid ID { get; set; }

        public string Email { get; set; }

        public EnumConfigurationType Type { get; set; }

        public string Value { get; set; }
    }
}
