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
    }
    
    public class RoomService : IRoomService
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        private readonly IReservation _reservation;

        public RoomService(IReservation reservation)
        {
            _reservation = reservation;
        }

        public Models.RoomService GetRoomServiceById(int id)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.RoomService WHERE id = @roomServiceId";

            var roomServiceDao = database.QuerySingle<RoomServiceDAO>(sql, new {roomServiceId = id});

            return TransformDaoToBusinessLogicRoomService(roomServiceDao);
        }

        private Models.RoomService TransformDaoToBusinessLogicRoomService(RoomServiceDAO roomServiceDao)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string reservationSql = "SELECT * FROM Hotel.Reservation WHERE id = @reservationId";
            var reservationDao =
                database.QuerySingle<ReservationDAO>(reservationSql,
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