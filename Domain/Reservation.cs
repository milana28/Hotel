using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Hotel.Models;

namespace Hotel.Domain
{
    public interface IReservation
    {
        Models.Reservation CheckIn(Models.Reservation reservation);
        Models.Reservation CheckOut(int guestId);
        Models.Reservation GetReservationById(int id);
        List<Models.Reservation> GetAll();
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
        
        public Models.Reservation CheckIn(Models.Reservation reservation)
        {
            var reservationDao = new ReservationDAO()
            {
                Id = GenerateReservationId(),
                CustomerId = reservation.Customer.Id,
                RoomNo = reservation.Room.RoomNo,
                CheckIn = reservation.CheckIn,
                CheckOut = reservation.CheckOut
            };
          
            if (CheckIfRoomExist(reservation.Room.RoomNo, reservation) == null || CheckIfCustomerExist(reservation.Customer.Id, reservation) == null)
            {
                return null;
            }
        
            using IDbConnection database = new SqlConnection(DatabaseConnectionString); 
            const string insertQuery = "INSERT INTO Hotel.Reservation VALUES (@customerId, @roomNo, @checkIn, @checkOut)";
            database.Execute(insertQuery, reservationDao);

            return TransformDaoToBusinessLogicReservation(reservationDao);
        }
        
        public Models.Reservation CheckOut(int reservationId)
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
            
            var reservation = database.QuerySingle<ReservationDAO>(sql, new {reservationId = id});

            return TransformDaoToBusinessLogicReservation(reservation);
        }

        public List<Models.Reservation> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var reservationsDao = database.Query<ReservationDAO>("SELECT * FROM Hotel.Reservation").ToList();
            var reservations = new List<Models.Reservation>();
            
            reservationsDao.ForEach(r => reservations.Add(TransformDaoToBusinessLogicReservation(r)));

            return reservations;
        }
        
        
        private Models.Reservation TransformDaoToBusinessLogicReservation(ReservationDAO reservationDao)
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
                CheckIn = reservationDao.CheckIn,
                CheckOut = reservationDao.CheckOut
            };
        }

        private Models.Room CheckIfRoomExist(int roomNo, Models.Reservation reservation)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var rooms = database.Query<Models.Room>("SELECT * FROM Hotel.Room").ToList();
            var room = _room.GetRoomByRoomNo(roomNo);
        
            var roomList = rooms.Where(r =>  r.RoomNo == reservation.Room.RoomNo && r.Location == reservation.Room.Location);
        
            return !roomList.Any() ? null : room;
        }
        
        private Models.Customer CheckIfCustomerExist(int customerId, Models.Reservation reservation)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var customers = database.Query<Models.Customer>("SELECT * FROM Hotel.Customer").ToList();
            var customer = _customer.GetCustomerById(customerId);
        
            var customerList = customers.Where(c => 
                c.Id == reservation.Customer.Id && 
                c.Name == reservation.Customer.Name && 
                c.Address == reservation.Customer.Address && 
                c.PhoneNo == reservation.Customer.PhoneNo);
        
            return !customerList.Any() ? null : customer;
        }

        private int GenerateReservationId()
        {
            var reservations = GetAll();
            var idList = new List<int>();
            reservations.ForEach(r => idList.Add(r.Id));
            var last = idList[^1];

            return last + 1;
        }
    }
}