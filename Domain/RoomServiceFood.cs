using System;
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
        List<Models.RoomServiceFood> GetRoomServiceFood(int? roomNo);
        Models.RoomServiceFood GetRoomServiceFoodById(int id);
        Models.RoomServiceFood TransformDaoToBusinessLogicRoomServiceFood(RoomServiceFoodDao roomServiceFoodDao);
        List<Models.Food> GetFoodForRoomService(int roomServiceId);
        Models.RoomServiceFood CreateRoomServiceFood(Models.RoomServiceFood roomServiceFood);
        List<Models.RoomServiceFood> GetRoomServiceFoodByReservationId(int? id);
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

        public Models.RoomServiceFood CreateRoomServiceFood(Models.RoomServiceFood roomServiceFood)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var roomServices = database.Query<Models.RoomService>("SELECT * FROM Hotel.RoomService").ToList();
            
            if (!roomServices.Contains(roomServiceFood.RoomService))
            {
                var roomServiceDao = new RoomServiceDao()
                {
                    ReservationId = roomServiceFood.RoomService.ReservationId,
                    Date = DateTime.Now
                };
                
                const string roomServiceQuery = "INSERT INTO Hotel.RoomService VALUES (@reservationId, @date); SELECT * FROM Hotel.RoomService WHERE id = SCOPE_IDENTITY()";
                database.Execute(roomServiceQuery, roomServiceDao);
            }
           
            var foodList = roomServiceFood.Food;
            foodList.ForEach(f =>
                {
                    if (CheckIfFoodExist(f.Id, f) == null)
                    {
                        return;
                    }
                    
                    var roomServiceFoodDao = new RoomServiceFoodDao()
                    {
                        RoomServiceId = roomServiceFood.RoomService.Id,
                        FoodId = f.Id
                    };

                    using IDbConnection connection = new SqlConnection(DatabaseConnectionString); 
                    const string insertQuery = "INSERT INTO Hotel.RoomService_Food VALUES (@roomServiceId, @foodId); SELECT * FROM Hotel.RoomService_Food WHERE id = SCOPE_IDENTITY()";
                    
                    connection.Execute(insertQuery, roomServiceFoodDao);
                });
            
            return roomServiceFood;
        }

        public List<Models.RoomServiceFood> GetRoomServiceFood(int? roomNo)
        {
            return roomNo == null ? GetAll() : GetRoomServiceFoodByRoomNo(roomNo);
        }

        public Models.RoomServiceFood GetRoomServiceFoodById(int id)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.RoomService_Food WHERE id = @roomServiceFoodId";

            var roomServiceFoodDao = database.QuerySingle<RoomServiceFoodDao>(sql, new {roomServiceFoodId = id});

            return TransformDaoToBusinessLogicRoomServiceFood(roomServiceFoodDao);
        }
        
        public Models.RoomServiceFood TransformDaoToBusinessLogicRoomServiceFood(RoomServiceFoodDao roomServiceFoodDao)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var roomService = _roomService.GetRoomServiceById(roomServiceFoodDao.RoomServiceId);
            var food = GetFoodForRoomService(roomServiceFoodDao.RoomServiceId);
            
            return new Models.RoomServiceFood()
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
            
            var roomServiceFood = database.Query<RoomServiceFoodDao>(sql, new {id = roomServiceId}).ToList();
            
            return roomServiceFood.Select(r => _food.GetFoodById(r.FoodId)).ToList();
        }
        
        public List<Models.RoomServiceFood> GetRoomServiceFoodByReservationId(int? id)
        {
            var roomServiceFoodList = new List<Models.RoomServiceFood>();
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
                
            const string roomServiceSql =
                "SELECT rs.* FROM Hotel.RoomService AS rs LEFT JOIN Hotel.Reservation AS r ON rs.reservationId = r.id WHERE r.id = @reservationId";
            var roomServices = database.Query<RoomServiceDao>(roomServiceSql, new {reservationId = id}).ToList();
                

            roomServices.ForEach(rs =>
            { 
                using IDbConnection connection = new SqlConnection(DatabaseConnectionString);
                const string sql = "SELECT * FROM Hotel.RoomService_Food WHERE roomServiceId = @id"; 
                var roomServiceFood = connection.QuerySingle<RoomServiceFoodDao>(sql, new {id = rs.Id}); 
                    
                roomServiceFoodList.Add(TransformDaoToBusinessLogicRoomServiceFood(roomServiceFood)); 
            });
                
            return roomServiceFoodList;
        }
        
        private List<Models.RoomServiceFood> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var roomServiceFoodDao = database.Query<RoomServiceFoodDao>("SELECT * FROM Hotel.RoomService_Food").ToList();
            
            var roomServiceFood = new List<Models.RoomServiceFood>();
            
            roomServiceFoodDao.ForEach(r => roomServiceFood.Add(TransformDaoToBusinessLogicRoomServiceFood(r)));

            return roomServiceFood;
        }

        private List<Models.RoomServiceFood> GetRoomServiceFoodByRoomNo(int? roomNo)
        {
            var roomServiceFoodList = new List<Models.RoomServiceFood>();
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
                
                const string roomServiceSql =
                    "SELECT rs.* FROM Hotel.RoomService AS rs LEFT JOIN Hotel.Reservation AS r ON rs.reservationId = r.id WHERE r.roomNo = @number";
                var roomServices = database.Query<RoomServiceDao>(roomServiceSql, new {number = roomNo}).ToList();
                

                roomServices.ForEach(rs =>
                { 
                    using IDbConnection connection = new SqlConnection(DatabaseConnectionString);
                    const string sql = "SELECT * FROM Hotel.RoomService_Food WHERE roomServiceId = @id"; 
                    var roomServiceFood = connection.QuerySingle<RoomServiceFoodDao>(sql, new {id = rs.Id}); 
                    
                    roomServiceFoodList.Add(TransformDaoToBusinessLogicRoomServiceFood(roomServiceFood)); 
                });
                
            return roomServiceFoodList;
        }

        private Models.Food CheckIfFoodExist(int foodId, Models.Food roomServiceFood)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var food = database.Query<Models.Food>("SELECT * FROM Hotel.Food").ToList();
            var foodById = _food.GetFoodById(foodId);
        
            var foodList = 
                food.Where(f =>  f.Id == roomServiceFood.Id && f.Name == roomServiceFood.Name && f.Price == roomServiceFood.Price);
        
            return !foodList.Any() ? null : foodById;
        }
        
        private int GenerateRoomServiceFoodId()
        {
            var roomServiceFood = GetAll();
            if (roomServiceFood.Count == 0)
            {
                return 1;
            }
            var idList = new List<int>();
            roomServiceFood.ForEach(r => idList.Add(r.Id));
            var last = idList.Max();

            return last + 1;
        }
    }
}