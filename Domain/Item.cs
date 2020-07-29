using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Hotel.Domain
{
    public interface IItem
    {
        Models.Item GetItemById(int id);
        List<Models.Item> GetAll();
    }
    
    public class Item : IItem
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        
        public Models.Item GetItemById(int id)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.Item WHERE id = @itemId";

            return database.QuerySingle<Models.Item>(sql, new {itemId = id});
        }
        
        public List<Models.Item> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            return database.Query<Models.Item>("SELECT * FROM Hotel.Item").ToList();
        }
    }
}