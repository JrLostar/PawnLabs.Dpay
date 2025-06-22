using PawnLabs.Dpay.Core.Enum;

namespace PawnLabs.Dpay.Api.Model.Response
{
    public class PaymentResponseModel
    {
        public Guid ID { get; set; }

        public string ProductName { get; set; }

        public string? ProductLogo { get; set; }

        public string BuyerAddress { get; set; }

        public string WalletAddress { get; set; }

        public float Price { get; set; }

        public EnumPriceType PriceType { get; set; }

        public DateTime Date { get; set; }
    }
}
