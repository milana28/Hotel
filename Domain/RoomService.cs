using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Hotel.Models;
namespace Hotel.Domain
{
    public interface IRoomService
    {
        Models.RoomService GetRoomServiceById(int id);
        List<Models.RoomService> GetAll();
        Models.RoomService CreateRoomService(RoomServiceDto roomServiceDto);
        List<Models.RoomService> GetRoomService(int? roomNo);
        List<Models.RoomService> GetRoomServiceByReservationId(int? id);
        List<Models.Item> GetItemForRoomService(int roomServiceId);
        Models.RoomService DeleteRoomService(int roomServiceId);
    }
    
    public class RoomService : IRoomService
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        private readonly IReservation _reservation;
        private readonly IItem _item;

        public RoomService(IReservation reservation, IItem item)
        {
            _reservation = reservation;
            _item = item;
        }

        public Models.RoomService CreateRoomService(RoomServiceDto roomServiceDto)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            
            if (CheckIfReservationExist(roomServiceDto.ReservationId) == null)
            {
                return null;
            }
            
            const string roomServiceQuery = "INSERT INTO Hotel.RoomService VALUES (@reservationId, @date); SELECT * FROM Hotel.RoomService WHERE id = SCOPE_IDENTITY()";
            var roomService = database.QueryFirst<RoomServiceDao>(roomServiceQuery, new {reservationId = roomServiceDto.ReservationId, date = DateTime.Now});
            
            var orderItems = new List<int>(roomServiceDto.OrderId);
            orderItems.ForEach(item =>
            {
                if (CheckIfItemExist(item) == null)
                {
                    return;
                }
                    
                var roomServiceFoodDao = new RoomServiceItemDao()
                {
                    RoomServiceId = roomService.Id,
                    ItemId = item
                };

                using IDbConnection connection = new SqlConnection(DatabaseConnectionString); 
                const string insertQuery = "INSERT INTO Hotel.RoomService_Item VALUES (@roomServiceId, @itemId)";
                connection.Execute(insertQuery, roomServiceFoodDao);
            });

            return TransformDaoToBusinessLogicRoomService(roomService);
        }
        
        public List<Models.RoomService> GetRoomService(int? roomNo)
        {
            return roomNo == null ? GetAll() : GetRoomServiceByRoomNo(roomNo);
        }
        
        public List<Models.RoomService> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var roomServicesDao = database.Query<RoomServiceDao>("SELECT * FROM Hotel.RoomService").ToList();
            
            var roomServices = new List<Models.RoomService>();
            
            roomServicesDao.ForEach(r => roomServices.Add(TransformDaoToBusinessLogicRoomService(r)));

            return roomServices;
        }

        public Models.RoomService GetRoomServiceById(int id)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.RoomService WHERE id = @roomServiceId";

            var roomServiceDao = database.QuerySingle<RoomServiceDao>(sql, new {roomServiceId = id});

            return TransformDaoToBusinessLogicRoomService(roomServiceDao);
        }
        
        public List<Models.RoomService> GetRoomServiceByReservationId(int? id)
        {
            var roomServiceList = new List<Models.RoomService>();
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
                
            const string roomServiceSql =
                "SELECT rs.* FROM Hotel.RoomService AS rs LEFT JOIN Hotel.Reservation AS r ON rs.reservationId = r.id WHERE r.id = @reservationId";
            var roomServices = database.Query<RoomServiceDao>(roomServiceSql, new {reservationId = id}).ToList();
                

            roomServices.ForEach(rs =>
            { 
                roomServiceList.Add(TransformDaoToBusinessLogicRoomService(rs));
            });
                
            return roomServiceList;
        }
        
        public List<Models.Item> GetItemForRoomService(int roomServiceId)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.RoomService_Item WHERE roomServiceId = @id";
            
            var roomServiceItem = database.Query<RoomServiceItemDao>(sql, new {id = roomServiceId}).ToList();
            
            return roomServiceItem.Select(r => _item.GetItemById(r.ItemId)).ToList();
        }
        
        public Models.RoomService DeleteRoomService(int roomServiceId)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "DELETE FROM Hotel.RoomService WHERE id = @id";
            
            database.Execute(sql, new {id = roomServiceId});

            return GetRoomServiceById(roomServiceId);
        }

        private Models.RoomService TransformDaoToBusinessLogicRoomService(RoomServiceDao roomServiceDao)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string reservationSql = "SELECT * FROM Hotel.Reservation WHERE id = @reservationId";
            
            var reservationDao =
                database.QuerySingle<ReservationDao>(reservationSql,
                    new {reservationId = roomServiceDao.ReservationId});
            var reservation = _reservation.TransformDaoToBusinessLogicReservation(reservationDao);
            
            const string foodQuery = "SELECT f.* FROM Hotel.RoomService_Item as r JOIN Hotel.Item as f ON r.itemId = f.id WHERE roomServiceId = @roomServiceId";
            var order = database.Query<Models.Item>(foodQuery, new {roomServiceId = roomServiceDao.Id}).ToList();

            return new Models.RoomService()
            {
                Id = roomServiceDao.Id,
                ReservationId = reservation.Id,
                Customer = reservation.Customer,
                Room = reservation.Room,
                Order = order,
                Date = DateTime.Now,
            };
        }
        
        private List<Models.RoomService> GetRoomServiceByRoomNo(int? roomNo)
        {
            var roomServiceList = new List<Models.RoomService>();
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
                
            const string roomServiceSql =
                "SELECT rs.* FROM Hotel.RoomService AS rs LEFT JOIN Hotel.Reservation AS r ON rs.reservationId = r.id WHERE r.roomNo = @number";
            var roomServices = database.Query<RoomServiceDao>(roomServiceSql, new {number = roomNo}).ToList();
                

            roomServices.ForEach(rs =>
            {
                roomServiceList.Add(TransformDaoToBusinessLogicRoomService(rs));
            });
                
            return roomServiceList;
        }
        
        private Models.Item CheckIfItemExist(int itemId)
        {
            return _item.GetItemById(itemId);
        }
        
        private Models.Reservation CheckIfReservationExist(int reservationId)
        {
            return _reservation.GetReservationById(reservationId);
        }
    }
}