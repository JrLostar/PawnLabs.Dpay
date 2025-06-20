using PawnLabs.Dpay.Core.Entity.Base;

namespace PawnLabs.Dpay.Data.Repository.Base
{
    public interface IBaseRepository<T> where T : IEntity
    {
        Task<T?> Get(T entity);

        Task<List<T>?> GetAll(T entity);

        Task<Guid?> Add(T entity);

        Task<bool> Update(T entity);

        Task<bool> Delete(T entity);
    }
}
