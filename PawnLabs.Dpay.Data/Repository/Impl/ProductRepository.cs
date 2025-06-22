using Dapper;
using Microsoft.Extensions.Configuration;
using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Data.Repository.Base.Impl;

namespace PawnLabs.Dpay.Data.Repository.Impl
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Product?> GetByID(Guid productID)
        {
            using (var connection = GetConnection())
            {
                var query = GetQuery("GetByID");

                return await connection.QueryFirstOrDefaultAsync<Product>(query, new { ID = productID });
            }
        }
    }
}
