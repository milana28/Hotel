using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Hotel.Domain
{
    public interface IRoom
    {
        Room GetRoomByRoomNo(int roomNo);
    }
    public class Room : IRoom
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        
        public Room GetRoomByRoomNo(int roomNo)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.Room WHERE roomNo = @number";

            return database.QuerySingle<Room>(sql, new {number = roomNo});
        }
    }
}