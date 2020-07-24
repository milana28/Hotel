using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Hotel.Models;

namespace Hotel.Domain
{
    public interface IRoomServiceFood
    {
        List<RoomService_Food> GetRoomServiceFood(int? roomNo);
        RoomService_Food GetRoomServiceFoodById(int id);
        RoomService_Food TransformDaoToBusinessLogicRoomServiceFood(RoomService_FoodDAO roomServiceFoodDao);
        List<Models.Food> GetFoodForRoomService(int roomServiceId);
        RoomService_Food CreateRoomServiceFood(RoomService_Food roomServiceFood);
        List<RoomService_Food> GetRoomServiceFoodByReservationId(int? id);
    }
    
    public class RoomServiceFood : IRoomServiceFood
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        private readonly IRoomService _roomService;
        private readonly IFood _food;

        public RoomServiceFood(IRoomService roomService, IFood food)
        {
            _roomService = roomService;
            _food = food;
        }

        public RoomService_Food CreateRoomServiceFood(RoomService_Food roomServiceFood)
        {
            return roomServiceFood;
        }

        public List<RoomService_Food> GetRoomServiceFood(int? roomNo)
        {
            return roomNo == null ? GetAll() : GetRoomServiceFoodByRoomNo(roomNo);
        }

        public RoomService_Food GetRoomServiceFoodById(int id)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.RoomService_Food WHERE id = @roomServiceFoodId";

            var roomServiceFoodDao = database.QuerySingle<RoomService_FoodDAO>(sql, new {roomServiceFoodId = id});

            return TransformDaoToBusinessLogicRoomServiceFood(roomServiceFoodDao);
        }
        
        public RoomService_Food TransformDaoToBusinessLogicRoomServiceFood(RoomService_FoodDAO roomServiceFoodDao)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var roomService = _roomService.GetRoomServiceById(roomServiceFoodDao.RoomServiceId);
            var food = GetFoodForRoomService(roomServiceFoodDao.RoomServiceId);
            
            return new RoomService_Food()
            {
                Id = roomServiceFoodDao.Id,
                RoomService = roomService,
                Food = food
            };
        }
        
        public List<Models.Food> GetFoodForRoomService(int roomServiceId)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.RoomService_Food WHERE roomServiceId = @id";
            
            var roomServiceFood = database.Query<RoomService_FoodDAO>(sql, new {id = roomServiceId}).ToList();
            
            return roomServiceFood.Select(r => _food.GetFoodById(r.FoodId)).ToList();
        }
        
        public List<RoomService_Food> GetRoomServiceFoodByReservationId(int? id)
        {
            var roomServiceFoodList = new List<RoomService_Food>();
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
                
            const string roomServiceSql =
                "SELECT rs.* FROM Hotel.RoomService AS rs LEFT JOIN Hotel.Reservation AS r ON rs.reservationId = r.id WHERE r.id = @reservationId";
            var roomServices = database.Query<RoomServiceDAO>(roomServiceSql, new {reservationId = id}).ToList();
                

            roomServices.ForEach(rs =>
            { 
                using IDbConnection connection = new SqlConnection(DatabaseConnectionString);
                const string sql = "SELECT * FROM Hotel.RoomService_Food WHERE roomServiceId = @id"; 
                var roomServiceFood = connection.QuerySingle<RoomService_FoodDAO>(sql, new {id = rs.Id}); 
                    
                roomServiceFoodList.Add(TransformDaoToBusinessLogicRoomServiceFood(roomServiceFood)); 
            });
                
            return roomServiceFoodList;
        }
        
        private List<RoomService_Food> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var roomServiceFoodDao = database.Query<RoomService_FoodDAO>("SELECT * FROM Hotel.RoomService_Food").ToList();
            
            var roomServiceFood = new List<RoomService_Food>();
            
            roomServiceFoodDao.ForEach(r => roomServiceFood.Add(TransformDaoToBusinessLogicRoomServiceFood(r)));

            return roomServiceFood;
        }

        private List<RoomService_Food> GetRoomServiceFoodByRoomNo(int? roomNo)
        {
            var roomServiceFoodList = new List<RoomService_Food>();
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
                
                const string roomServiceSql =
                    "SELECT rs.* FROM Hotel.RoomService AS rs LEFT JOIN Hotel.Reservation AS r ON rs.reservationId = r.id WHERE r.roomNo = @number";
                var roomServices = database.Query<RoomServiceDAO>(roomServiceSql, new {number = roomNo}).ToList();
                

                roomServices.ForEach(rs =>
                { 
                    using IDbConnection connection = new SqlConnection(DatabaseConnectionString);
                    const string sql = "SELECT * FROM Hotel.RoomService_Food WHERE roomServiceId = @id"; 
                    var roomServiceFood = connection.QuerySingle<RoomService_FoodDAO>(sql, new {id = rs.Id}); 
                    
                    roomServiceFoodList.Add(TransformDaoToBusinessLogicRoomServiceFood(roomServiceFood)); 
                });
                
            return roomServiceFoodList;
        }
    }
}