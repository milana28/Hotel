using System.Collections.Generic;
using Hotel.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationController : ControllerBase
    {

        private readonly IReservation _reservation;

        public ReservationController(IReservation reservation)
        {
            _reservation = reservation;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public ActionResult<Models.Reservation> CreateReservation(Models.Reservation reservation)
        {
            return _reservation.CheckIn(reservation);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Models.Reservation>> GetCustomers()
        {
            return _reservation.GetAll();
        }

        [HttpGet("{reservationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Reservation> GetReservation(int reservationId)
        {
            var reservation = _reservation.GetReservationById(reservationId);
            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        [HttpDelete("{reservationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Reservation> DeleteReservation(int reservationId)
        {
            var reservation = _reservation.GetReservationById(reservationId);

            if (reservation == null)
            {
                return NotFound();
            }

            return _reservation.CheckOut(reservationId);
        }
    }
}