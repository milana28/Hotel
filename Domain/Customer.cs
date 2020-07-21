using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Domain
{
    public interface ICustomer
    {
        Models.Customer CreateCustomer(Models.Customer guest);
        Models.Customer DeleteCustomer(int guestId);
        Models.Customer GetCustomerById(int id);
        List<Models.Customer> GetAll();

    }
    
    public class Customer : ICustomer
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        private readonly List<Models.Customer> _customers = new List<Models.Customer>();
        private readonly IRoom _room;

        public Customer(IRoom room)
        {
            _room = room;
        }

        public Models.Customer CreateCustomer(Models.Customer guest)
        {
            var guestDao = new CustomerDAO()
            {
                Name = guest.Name,
                PhoneNo = guest.PhoneNo,
                Address = guest.Address,
                RoomNo = guest.Room.RoomNo
            };

            if (CheckIfRoomExist(guest.Room.RoomNo, guest) == null)
            {
                return null;
            }
        
            using IDbConnection database = new SqlConnection(DatabaseConnectionString); 
            const string insertQuery = "INSERT INTO Hotel.Customer VALUES (@name, @phoneNo, @address, @roomNo)";
            database.Execute(insertQuery, guestDao);

            return TransformDaoToBusinessLogicCustomer(guestDao);
        }

        public Models.Customer DeleteCustomer(int guestId)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "DELETE FROM Hotel.Customer WHERE id = @customerId";
            
            database.Execute(sql, new {customerId = guestId});

            return GetCustomerById(guestId);;
        }
        
        public Models.Customer GetCustomerById(int id)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.Customer WHERE id = @customerId";
            
            var customerDao = database.QuerySingle<CustomerDAO>(sql, new {customerId = id});

            return TransformDaoToBusinessLogicCustomer(customerDao);
        }

        public List<Models.Customer> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var customerDaoList = database.Query<CustomerDAO>("SELECT * FROM Hotel.Customer").ToList();

            customerDaoList.ForEach(e => _customers.Add(TransformDaoToBusinessLogicCustomer(e)));

            return _customers;
        }

        private Models.Customer TransformDaoToBusinessLogicCustomer(CustomerDAO customerDao)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.Room WHERE roomNo = @roomNo";
            var room = database.QuerySingle<Models.Room>(sql, new {roomNo = customerDao.RoomNo});

            return new Models.Customer()
            {
                Id = customerDao.Id,
                Name = customerDao.Name,
                PhoneNo = customerDao.PhoneNo,
                Address = customerDao.Address,
                Room = room
            };
        }

        private Room CheckIfRoomExist(int roomNo, Models.Customer customer)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var rooms = database.Query<Models.Room>("SELECT * FROM Hotel.Room").ToList();
            var room = _room.GetRoomByRoomNo(roomNo);

            var roomList = rooms.Where(r => r.RoomNo == roomNo && r.Location == customer.Room.Location);

            return !roomList.Any() ? null : room;
        }
    }
}
