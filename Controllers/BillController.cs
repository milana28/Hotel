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
        private readonly IReservation _reservation;

        public BillController(IBill bill, IReservation reservation)
        {
            _bill = bill;
            _reservation = reservation;
        }

        // [HttpGet]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // public ActionResult<Models.Bill> GenerateBill([FromQuery(Name = "reservationId")] int reservationId)
        // {
        //        var reservation = _reservation.GetReservationById(reservationId);
        //        if (reservation == null)
        //        {
        //            return NotFound();
        //        }
               
        //     return reservation;
        // }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Bill> GenerateBill([FromQuery(Name = "reservationId")] int reservationId)
        {
            var priceOfRoom = _bill.GenerateBill(reservationId).PriceOfRoom;
            var priceOfRoomService = _bill.GenerateBill(reservationId).PriceOfRoomService;
            var priceWithoutVat = _bill.GenerateBill(reservationId).PriceWithoutVat;
            var totalPrice = _bill.GenerateBill(reservationId).TotalPrice;
            var reservation = _reservation.GetReservationById(reservationId);

            if (reservation == null)
            {
                return NotFound("No reservation found with this ID!");
            }
            
            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int) HttpStatusCode.OK,
                Content = @"<html><body>
                          <table style='border-collapse: collapse; width: 100%; height: 60px'>
                          <tr>
                          <td style='border: 1px solid black; width: 20%; padding-left: 10px'>Reservation ID</td>
                          <td style='border: 1px solid black; width: 20%; padding-left: 10px'>Price of room</td>
                          <td style='border: 1px solid black; width: 20%; padding-left: 10px'>Price of room service</td>
                          <td style='border: 1px solid black; width: 20%; padding-left: 10px'>Price without VAT</td>
                          <td style='border: 1px solid black; width: 20%; padding-left: 10px'>Total price</td>
                          </tr>
                          <tr>" +
                          "<td style='border: 1px solid black; width: 20%; padding-left: 10px'>" + reservationId + "</td>" +
                          "<td style='border: 1px solid black; width: 20%; padding-left: 10px'>" + priceOfRoom + "</td>" +
                          "<td style='border: 1px solid black; width: 20%; padding-left: 10px'>" + priceOfRoomService + "</td>" +
                          "<td style='border: 1px solid black; width: 20%; padding-left: 10px'>" + priceWithoutVat + "</td>" +
                          "<td style='border: 1px solid black; width: 20%; padding-left: 10px'>" + totalPrice + "</td>" +
                          @"</tr>
                          </table>
                          </body></html>"
            };
        }
    }
}