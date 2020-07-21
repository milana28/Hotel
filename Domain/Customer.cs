using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Hotel.Models;
namespace Hotel.Domain
{
    public interface ICustomer
    {
        Models.Customer CheckIn(Models.Customer guest);

    }
    
    public class Customer : ICustomer
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";

        public Models.Customer CheckIn(Models.Customer guest)
        {
            var newGuest = new CustomerDAO()
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
            database.Execute(insertQuery, newGuest);

            return TransformDaoToBusinessLogicCustomer(newGuest);
        }

        private Models.Customer TransformDaoToBusinessLogicCustomer(CustomerDAO customerDao)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.Room WHERE roomNo = @roomNo";
            var room = database.QuerySingle<Models.Room>(sql, new {roomNo = customerDao.RoomNo});

            return new Models.Customer()
            {
                Name = customerDao.Name,
                PhoneNo = customerDao.PhoneNo,
                Address = customerDao.Address,
                Room = room
            };
        }

        private Room GetRoomByRoomNo(int roomNo)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.Room WHERE roomNo = @number";

            return database.QuerySingle<Room>(sql, new {number = roomNo});
        }

        private Room CheckIfRoomExist(int roomNo, Models.Customer customer)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var rooms = database.Query<Models.Room>("SELECT * FROM Hotel.Room").ToList();
            var room = GetRoomByRoomNo(roomNo);

            var roomList = rooms.Where(r => r.RoomNo == roomNo && r.Location == customer.Room.Location);

            return !roomList.Any() ? null : room;
        }
    }
}
