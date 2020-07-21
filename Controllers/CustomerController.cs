using System.Collections.Generic;
using Hotel.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        
        private readonly ICustomer _customer;

        public CustomerController(ICustomer customer)
        {
            _customer = customer;
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public ActionResult<Models.Customer> CreateCustomer(Models.Customer customer)
        {
            return _customer.CreateCustomer(customer);
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Models.Customer>> GetCustomers()
        {
            return _customer.GetAll();
        }
        
        [HttpGet("{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Customer> GetCustomerInfo(int customerId)
        {
            var customer = _customer.GetCustomerById(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Customer> DeleteCustomer(int guestId)
        {
            var guest = _customer.GetCustomerById(guestId);
            
            if (guest == null)
            {
                return NotFound();
            }
            
            return _customer.DeleteCustomer(guestId);
        }
      
    }
}