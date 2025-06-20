using PawnLabs.Dpay.Core.Entity.Base;
using PawnLabs.Dpay.Core.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Dapper;

namespace PawnLabs.Dpay.Data.Repository.Base.Impl
{
    public class BaseRepository<T> : IBaseRepository<T> where T : IEntity
    {
        private readonly IConfiguration _configuration;

        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected IDbConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        protected string GetQuery(string queryId)
        {
            var xmlPath = Path.Combine(_configuration.GetSection("QueryFolderPath").Value, @$"{typeof(T).Name}.xml");

            XDocument xDoc = XDocument.Load(xmlPath);

            foreach (XElement queryElement in xDoc.Descendants("Query"))
            {
                if (queryElement.Attribute("id").Value == queryId)
                {
                    return queryElement.Value;
                }
            }

            return string.Empty;
        }

        public async Task<T?> Get(T entity)
        {
            using (var connection = GetConnection())
            {
                var query = GetQuery("Get");

                return await connection.QueryFirstOrDefaultAsync<T>(query, entity);
            }
        }

        public async Task<List<T>?> GetAll(T entity)
        {
            using (var connection = GetConnection())
            {
                var query = GetQuery("GetAll");

                return (await connection.QueryAsync<T>(query, entity))?.ToList();
            }
        }

        public async Task<Guid?> Add(T entity)
        {
            using (var connection = GetConnection())
            {
                var query = GetQuery("Add");

                return await connection.ExecuteScalarAsync<Guid?>(query, entity);
            }
        }

        public async Task<bool> Update(T entity)
        {
            using (var connection = GetConnection())
            {
                var query = GetQuery("Update");

                return (await connection.ExecuteAsync(query, entity)) > 0;
            }
        }

        public async Task<bool> Delete(T entity)
        {
            using (var connection = GetConnection())
            {
                var query = GetQuery("Delete");

                return (await connection.ExecuteAsync(query, entity)) > 0;
            }
        }
    }
}
