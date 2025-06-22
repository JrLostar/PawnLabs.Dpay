using Microsoft.Extensions.DependencyInjection;
using PawnLabs.Dpay.Core.Entity.Base;
using PawnLabs.Dpay.Data.Repository.Base;

namespace PawnLabs.Dpay.Business.Service.Base.Impl
{
    public class BaseService<T> : IBaseService<T> where T : IEntity
    {
        private IBaseRepository<T> Repository => _serviceProvider.GetService<IBaseRepository<T>>();

        private readonly IServiceProvider _serviceProvider;

        public BaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<T?> Get(T entity) => await Repository.Get(entity);

        public async Task<List<T>?> GetAll(T entity) => await Repository.GetAll(entity);

        public async Task<Guid?> Add(T entity) => await Repository.Add(entity);

        public async Task<bool> Update(T entity) => await Repository.Update(entity);

        public async Task<bool> Delete(T entity) => await Repository.Delete(entity);
    }
}
