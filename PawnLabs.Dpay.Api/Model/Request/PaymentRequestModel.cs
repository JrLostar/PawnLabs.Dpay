using PawnLabs.Dpay.Core.Enum;

namespace PawnLabs.Dpay.Api.Model.Request
{
    public class PaymentRequestModel
    {
        public Guid ProductID { get; set; }

        public string BuyerAddress { get; set; }

        public string WalletAddress { get; set; }

        public float Price { get; set; }

        public EnumPriceType PriceType { get; set; }
    }
}
