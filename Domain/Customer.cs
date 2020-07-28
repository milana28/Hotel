using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

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

        public Models.Customer CreateCustomer(Models.Customer customer)
        {
            var newCustomer = new Models.Customer()
            {
                Name = customer.Name,
                PhoneNo = customer.PhoneNo,
                Address = customer.Address,
            };
            
            using IDbConnection database = new SqlConnection(DatabaseConnectionString); 
            const string insertQuery = "INSERT INTO Hotel.Customer VALUES (@name, @phoneNo, @address); SELECT * FROM Hotel.Customer WHERE id = SCOPE_IDENTITY()";
                
            return database.QueryFirst<Models.Customer>(insertQuery, newCustomer);
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
            
            var customer = database.QuerySingle<Models.Customer>(sql, new {customerId = id});

            return customer;
        }

        public List<Models.Customer> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
           
            return database.Query<Models.Customer>("SELECT * FROM Hotel.Customer").ToList();
        }
    }
}
