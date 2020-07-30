using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Hotel.Models;

namespace Hotel.Domain
{
    public interface IRoom
    {
        Models.Room GetRoomByRoomNo(int roomNo);
        List<Models.Room> GetAll();
    }
    public class Room : IRoom
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        
        public Models.Room GetRoomByRoomNo(int roomNo)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.Room WHERE roomNo = @number";

            var room =  database.QuerySingle<Models.Room>(sql, new {number = roomNo});

            return room;
        }
        
        public List<Models.Room> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            return  database.Query<Models.Room>("SELECT * FROM Hotel.Room").ToList();
        }
    }
}