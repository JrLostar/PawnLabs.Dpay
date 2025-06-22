using Dapper;
using Microsoft.Extensions.Configuration;
using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Data.Repository.Base.Impl;

namespace PawnLabs.Dpay.Data.Repository.Impl
{
    public class ConfigurationRepository : BaseRepository<Configuration>, IConfigurationRepository
    {
        public ConfigurationRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> UpdateByEmail(Configuration entity)
        {
            using (var connection = GetConnection())
            {
                var query = GetQuery("UpdateByEmail");

                return (await connection.ExecuteAsync(query, entity)) > 0;
            }
        }

        public async Task<bool> DeleteAll(string email)
        {
            using (var connection = GetConnection())
            {
                var query = GetQuery("DeleteAll");

                return (await connection.ExecuteAsync(query, new { Email = email })) > 0;
            }
        }
    }
}
