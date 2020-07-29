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
            var priceOfRoom = _bill.GenerateBill(reservationId).PriceOfRoom;
            var priceOfRoomService = _bill.GenerateBill(reservationId).PriceOfRoomService;
            var priceWithoutPdv = _bill.GenerateBill(reservationId).PriceWithoutPdv;
            var totalPrice = _bill.GenerateBill(reservationId).TotalPrice;
            
            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int) HttpStatusCode.OK,
                Content = "<html><body>" +
                          "<table style='border-collapse: collapse; width: 100%; height: 60px'>" +
                          "<tr>" +
                          "<td style='border: 1px solid black; width: 25%; padding-left: 10px'>Price of room</td>" +
                          "<td style='border: 1px solid black; width: 25%; padding-left: 10px'>Price of room service</td>" +
                          "<td style='border: 1px solid black; width: 25%; padding-left: 10px'>Price without PDV</td>" +
                          "<td style='border: 1px solid black; width: 25%; padding-left: 10px'>Total price</td>" +
                          "</tr>" +
                          "<tr>" +
                          "<td style='border: 1px solid black; width: 25%; padding-left: 10px'>" + priceOfRoom + "</td>" +
                          "<td style='border: 1px solid black; width: 25%; padding-left: 10px'>" + priceOfRoomService + "</td>" +
                          "<td style='border: 1px solid black; width: 25%; padding-left: 10px'>" + priceWithoutPdv + "</td>" +
                          "<td style='border: 1px solid black; width: 25%; padding-left: 10px'>" + totalPrice + "</td>" +
                          "</tr>" +
                          "</table>" +
                          "</body></html>"
            };
        }
    }
}