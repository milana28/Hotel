using System.Collections.Generic;
using Hotel.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BillController : ControllerBase
    {
        private readonly IBill _bill;

        public BillController(IBill bill)
        {
            _bill = bill;
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public ActionResult<Models.Bill> CreateBill(Models.Bill bill)
        {
            return _bill.CreateBill(bill);
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Models.Bill>> GetAll([FromQuery(Name = "reservationId")] int? reservationId)
        {
            return _bill.GetBills(reservationId);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Bill> GetBillById(int id)
        {
            var bill = _bill.GetBillById(id);
            if (bill == null)
            {
                return NotFound();
            }

            return bill;
        }
    }
}