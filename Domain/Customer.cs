using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Hotel.Models;

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
                Id = GenerateCustomerId(),
                Name = customer.Name,
                PhoneNo = customer.PhoneNo,
                Address = customer.Address,
            };
            
            using IDbConnection database = new SqlConnection(DatabaseConnectionString); 
            const string insertQuery = "INSERT INTO Hotel.Customer VALUES (@name, @phoneNo, @address)";
            database.Execute(insertQuery, newCustomer);

            return newCustomer;
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
        
        private int GenerateCustomerId()
        {
            var customers = GetAll();
            if (customers.Count == 0)
            {
                return 1;
            }
            var idList = new List<int>();
            customers.ForEach(c => idList.Add(c.Id));
            var last = idList.Max();

            return last + 1;
        }
    }
}
