using System.Net;
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

        // [HttpGet]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // public ActionResult<Models.Bill> GenerateBill([FromQuery(Name = "reservationId")] int reservationId)
        // {
        //     return _bill.GenerateBill(reservationId);
        // }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Bill> GenerateBill([FromQuery(Name = "reservationId")] int reservationId)
        {
            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int) HttpStatusCode.OK,
                Content = "<html><body>" +
                          "<table>" +
                          "<tr>" +
                          "<td>Price of room</td>" +
                          "<td>Price of Room service</td>" +
                          "<td>Price without PDV</td>" +
                          "<td>Total price</td>" +
                          "</tr>" +
                          "<tr>" +
                          "<td>Price of room</td>" +
                          "<td>Price of room</td>" +
                          "<td>Price of room</td>" +
                          "<td>Price of room</td>" +
                          "</tr>" +
                          "</table>" +
                          "</body></html>"
            };
        }
    }
}