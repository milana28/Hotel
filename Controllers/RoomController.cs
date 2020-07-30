using System.Collections.Generic;
using Hotel.Domain;
using Hotel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoom _room;
        private readonly IReservation _reservation;

        public RoomController(IRoom room, IReservation reservation)
        {
            _room = room;
            _reservation = reservation;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<RoomDto>> GetRooms()
        {
            return _reservation.GetRooms();
        }
    }
}