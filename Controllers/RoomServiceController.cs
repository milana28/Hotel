using System.Collections.Generic;
using Hotel.Domain;
using Hotel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomServiceController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomServiceController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public ActionResult<Models.RoomService> CreateReservation(RoomServiceDto roomServiceDto)
        {
            return _roomService.CreateRoomService(roomServiceDto);
        }
        
        [HttpGet] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Models.RoomService>> GetRoomService([FromQuery(Name = "room")] int? roomNo)
        {
            return _roomService.GetRoomService(roomNo);
        }
        
        [HttpGet("{id}")] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.RoomService> GetRoomServiceById(int id)
        {
            var roomService = _roomService.GetRoomServiceById(id);
            if (roomService == null)
            {
                return NotFound("No room service found with this ID!");
            }

            return roomService;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.RoomService> DeleteRoomService(int id)
        {
            var roomService = _roomService.DeleteRoomService(id);
            if (roomService == null)
            {
                return NotFound("No room service found with this ID!");
            }

            return roomService;
        }
    }
}