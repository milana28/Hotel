using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using Dapper;
using Hotel.Models;

namespace Hotel.Domain
{
    public interface IReservation
    {
        Models.Reservation CreateReservation(ReservationDao reservationDao);
        Models.Reservation DeleteReservation(int guestId);
        Models.Reservation GetReservationById(int id);
        List<Models.Reservation> GetReservations(int? roomNo, string? name);
        Models.Reservation CheckIn(int reservationId);
        Models.Reservation CheckOut(int reservationId);
        Models.Reservation TransformDaoToBusinessLogicReservation(ReservationDao reservationDao);
        List<Models.Reservation> GetReservationsByRoom(int? roomNo);
        List<RoomDto> GetRooms();
    }
    
    public class Reservation : IReservation
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        private readonly IRoom _room;
        private readonly ICustomer _customer;
    
        public Reservation(IRoom room, ICustomer customer)
        {
            _room = room;
            _customer = customer;
        }
        
        public Models.Reservation CreateReservation(ReservationDao reservation)
        {
            var reservationDao = new ReservationDao()
            {
                CustomerId = reservation.CustomerId,
                RoomNo = reservation.RoomNo,
                Date = DateTime.Now.Date,
                CheckInDate = null,
                CheckOutDate = null
            };
                
            if (CheckIfRoomExist(reservationDao.RoomNo) == null || CheckIfCustomerExist(reservationDao.CustomerId) == null)
            {
                return null;
            }
        
            using IDbConnection database = new SqlConnection(DatabaseConnectionString); 
            const string insertQuery = "INSERT INTO Hotel.Reservation VALUES (@customerId, @roomNo, @date, @checkInDate, @checkOutDate); SELECT * FROM Hotel.Reservation WHERE id = SCOPE_IDENTITY()";

            return TransformDaoToBusinessLogicReservation(database.QueryFirst<ReservationDao>(insertQuery, reservationDao));
        }
        
        public Models.Reservation CheckIn(int reservationId)
        {
            var reservation = GetReservationById(reservationId);

            if (GetReservationIfRoomNotAvailable(reservation) != null)
            {
                return null;
            }

            using IDbConnection database = new SqlConnection(DatabaseConnectionString); 
            const string insertQuery = "UPDATE Hotel.Reservation SET checkInDate = @checkInDate WHERE id = @id; SELECT * FROM Hotel.Reservation WHERE id = @id";

            return TransformDaoToBusinessLogicReservation(database.QueryFirst<ReservationDao>(insertQuery, new {checkInDate = DateTime.Now.Date, id = reservation.Id}));
        }
        
        public Models.Reservation CheckOut(int reservationId)
        {
            var reservation = GetReservationById(reservationId);
            
            if (reservation.CheckInDate == null)
            {
                return null;
            }
        
            using IDbConnection database = new SqlConnection(DatabaseConnectionString); 
            const string insertQuery = "UPDATE Hotel.Reservation SET checkOutDate = @checkOutDate WHERE id = @id; SELECT * FROM Hotel.Reservation WHERE id = @id";
            
            return TransformDaoToBusinessLogicReservation(database.QueryFirst<ReservationDao>(insertQuery, new {checkOutDate = DateTime.Now.Date, id = reservation.Id}));
        }
        
        public Models.Reservation DeleteReservation(int reservationId)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "DELETE FROM Hotel.Reservation WHERE id = @id";
            
            database.Execute(sql, new {id = reservationId});

            return GetReservationById(reservationId);
        }
        
        public Models.Reservation GetReservationById(int id)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.Reservation WHERE id = @reservationId";
            
            var reservation = database.QuerySingle<ReservationDao>(sql, new {reservationId = id});

            return TransformDaoToBusinessLogicReservation(reservation);
        }


        public List<Models.Reservation> GetReservations(int? roomNo, string? name)
        {
            if (name != null)
            {
                return GeReservationByCustomerName(name);
            }
            
            return roomNo == null ? GetAll() : GetReservationsByRoom(roomNo);
        }
        
        public Models.Reservation TransformDaoToBusinessLogicReservation(ReservationDao reservationDao)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            
            const string roomQuery = "SELECT * FROM Hotel.Room WHERE roomNo = @roomNo";
            var room = database.QuerySingle<Models.Room>(roomQuery, new {roomNo = reservationDao.RoomNo});
            
            const string customerQuery = "SELECT * FROM Hotel.Customer WHERE id = @customerId";
            var customer = database.QuerySingle<Models.Customer>(customerQuery, new {customerId = reservationDao.CustomerId});
        
            return new Models.Reservation()
            {
                Id = reservationDao.Id,
                Customer = customer,
                Room = room,
                Date = reservationDao.Date, 
                CheckInDate = reservationDao.CheckInDate,
                CheckOutDate = reservationDao.CheckOutDate
            };
        }
        
        public List<Models.Reservation> GetReservationsByRoom(int? roomNo)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.Reservation WHERE roomNo = @number";
            
            var reservationsDao = database.Query<ReservationDao>(sql, new {number = roomNo}).ToList();
           
            var reservations = new List<Models.Reservation>();
            reservationsDao.ForEach(r => reservations.Add(TransformDaoToBusinessLogicReservation(r)));
            
            return reservations;
        }
        
        public List<RoomDto> GetRooms()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var rooms =  database.Query<Models.Room>("SELECT * FROM Hotel.Room").ToList();
            var roomsDto = new List<RoomDto>();

            rooms.ForEach(room =>
            {
                if (GetCurrentReservationForRoom(room.RoomNo) != null)
                {
                    roomsDto.Add(new RoomDto()
                    {
                        RoomNo = room.RoomNo,
                        Location = room.Location,
                        PricePerDay = room.PricePerDay,
                        Available = false
                    });
                }

                if (GetCurrentReservationForRoom(room.RoomNo) == null)
                {
                    roomsDto.Add(new RoomDto()
                    {
                        RoomNo = room.RoomNo,
                        Location = room.Location,
                        PricePerDay = room.PricePerDay,
                        Available = true
                    });
                }
            });
         
            return roomsDto;
        }

        // public List<Models.Room> GetAvailableRoomsForDate(DateTime date)
        // {
        //     var reservations = GetAll();
        //     reservations.FindAll(r => )
        //
        // }

        private Models.Reservation GetCurrentReservationForRoom(int roomNo)
        {
            var reservationsForRoom = GetReservationsByRoom(roomNo);
            return reservationsForRoom.Find(r => r.CheckOutDate == null);
        }
     
        private Models.Reservation GetReservationIfRoomNotAvailable(Models.Reservation reservation)
        {
            var roomNo = reservation.Room.RoomNo;
            var reservations = GetReservationsByRoom(roomNo);

            return reservations.FirstOrDefault(r =>
                r.CheckInDate != null && r.CheckInDate <= DateTime.Now && r.CheckOutDate == null);
        }

        private List<Models.Reservation> GeReservationByCustomerName(string? name)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT r.* FROM Hotel.Reservation as r LEFT JOIN Hotel.Customer as c ON r.CustomerId = c.Id WHERE c.Name = @customerName";
            
            var reservationsDao = database.Query<ReservationDao>(sql, new {customerName = name}).ToList();
           
            var reservations = new List<Models.Reservation>();
            reservationsDao.ForEach(r => reservations.Add(TransformDaoToBusinessLogicReservation(r)));
            
            return reservations;
        }
        
        private List<Models.Reservation> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var reservationsDao = database.Query<ReservationDao>("SELECT * FROM Hotel.Reservation").ToList();
            var reservations = new List<Models.Reservation>();
            
            reservationsDao.ForEach(r => reservations.Add(TransformDaoToBusinessLogicReservation(r)));

            return reservations;
        }

        private Models.Room CheckIfRoomExist(int roomNo)
        {
            return _room.GetRoomByRoomNo(roomNo);
        }
        
        private Models.Customer CheckIfCustomerExist(int customerId)
        {
            return _customer.GetCustomerById(customerId);
        }
    }
}