using PawnLabs.Dpay.Core.Entity.Base;
using PawnLabs.Dpay.Core.Enum;

namespace PawnLabs.Dpay.Core.Entity
{
    public class Payment : IEntity
    {
        public Guid ID { get; set; }

        public string Email { get; set; }

        public int ProductID { get; set; }

        public string BuyerAddress { get; set; }

        public string WalletAddress { get; set; }

        public float Price { get; set; }

        public EnumPriceType PriceType { get; set; }

        public DateTime Date { get; set; }
    }
}
