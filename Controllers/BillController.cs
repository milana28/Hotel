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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Bill> GenerateBill([FromQuery(Name = "reservationId")] int reservationId)
        {
            return _bill.GenerateBill(reservationId);
        }
    }
}