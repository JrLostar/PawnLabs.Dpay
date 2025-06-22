using PawnLabs.Dpay.Business.Service.Base.Impl;
using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Data.Repository;

namespace PawnLabs.Dpay.Business.Service.Impl
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private IProductRepository _productRepository;

        public ProductService(IServiceProvider serviceProvider, IProductRepository productRepository) : base(serviceProvider)
        {
            _productRepository = productRepository;
        }

        public async Task<Product?> GetByID(Guid productID)
        {
            return await _productRepository.GetByID(productID);
        }
    }
}
