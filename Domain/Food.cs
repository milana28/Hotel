using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Hotel.Domain
{
    public interface IFood
    {
        Models.Food GetFoodById(int id);
        List<Models.Food> GetAll();
    }
    
    public class Food : IFood
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        
        public Models.Food GetFoodById(int id)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.Food WHERE id = @foodId";

            return database.QuerySingle<Models.Food>(sql, new {foodId = id});
        }
        
        public List<Models.Food> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            return database.Query<Models.Food>("SELECT * FROM Hotel.Food").ToList();
        }
    }
}