using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Data.Repository.Base;

namespace PawnLabs.Dpay.Data.Repository
{
    public interface IConfigurationRepository : IBaseRepository<Configuration>
    {
        Task<bool> UpdateByEmail(Configuration entity);

        Task<bool> DeleteAll(string email);
    }
}
