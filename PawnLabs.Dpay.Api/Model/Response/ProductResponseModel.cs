using PawnLabs.Dpay.Core.Enum;

namespace PawnLabs.Dpay.Api.Model.Response
{
    public class ProductResponseModel
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public float Price { get; set; }

        public EnumPriceType PriceType { get; set; }

        public string? Logo { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
