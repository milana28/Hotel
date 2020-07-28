using System;
using System.Collections.Generic;
using Hotel.Domain;
using Hotel.Models;
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
        public ActionResult<Models.Reservation> CreateReservation(ReservationDao reservationDao)
        {
            return _reservation.CreateReservation(reservationDao);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Models.Reservation>> GetCustomers([FromQuery(Name = "room")] int? roomNo, [FromQuery(Name = "customerName")] string? name)
        {
            return _reservation.GetReservations(roomNo, name);
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

            return _reservation.DeleteReservation(reservationId);
        }
        
        [HttpPut("{reservationId}/checkIn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Reservation> CheckIn(int reservationId)
        {
            try
            {
                var reservation = _reservation.GetReservationById(reservationId);
                if (reservation == null)
                {
                    return NotFound();
                }

                return _reservation.CheckIn(reservationId);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e}");
            }
        }
        
        [HttpPut("{reservationId}/checkOut")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Reservation> CheckOut(int reservationId)
        {
            try
            {
                var reservation = _reservation.GetReservationById(reservationId);
                if (reservation == null)
                {
                    return NotFound();
                }

                return _reservation.CheckOut(reservationId);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e}");
            }
        }
    }
}