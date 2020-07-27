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
    }
    
    public class RoomService : IRoomService
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        private readonly IReservation _reservation;

        public RoomService(IReservation reservation)
        {
            _reservation = reservation;
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

        private Models.RoomService TransformDaoToBusinessLogicRoomService(RoomServiceDao roomServiceDao)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string reservationSql = "SELECT * FROM Hotel.Reservation WHERE id = @reservationId";
            
            var reservationDao =
                database.QuerySingle<ReservationDao>(reservationSql,
                    new {reservationId = roomServiceDao.ReservationId});
            var reservation = _reservation.TransformDaoToBusinessLogicReservation(reservationDao);

            return new Models.RoomService()
            {
                Id = roomServiceDao.Id,
                ReservationId = reservation.Id,
                Customer = reservation.Customer,
                Room = reservation.Room,
                Date = DateTime.Now,
            };
        }
    }
}