using PawnLabs.Dpay.Business.Service.Base;
using PawnLabs.Dpay.Core.Entity;

namespace PawnLabs.Dpay.Business.Service
{
    public interface IProductService : IBaseService<Product>
    {
        Task<Product?> GetByID(Guid productID);
    }
}
