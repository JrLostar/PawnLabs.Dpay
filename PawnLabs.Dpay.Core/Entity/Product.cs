using PawnLabs.Dpay.Core.Entity.Base;
using PawnLabs.Dpay.Core.Enum;

namespace PawnLabs.Dpay.Core.Entity
{
    public class Product : IEntity
    {
        public Guid ID { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public float Price { get; set; }

        public EnumPriceType PriceType { get; set; }

        public string? Logo { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
