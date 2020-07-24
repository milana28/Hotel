using System.Collections.Generic;
using Hotel.Domain;
using Hotel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomServiceFoodController : ControllerBase
    {
        private readonly IRoomServiceFood _roomServiceFood;

        public RoomServiceFoodController(IRoomServiceFood roomServiceFood)
        {
            _roomServiceFood = roomServiceFood;
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public ActionResult<RoomService_Food> CreateReservation(RoomService_Food roomServiceFood)
        {
            return _roomServiceFood.CreateRoomServiceFood(roomServiceFood);
        }
        
        [HttpGet] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<RoomService_Food>> GetRoomServiceFood([FromQuery(Name = "room")] int? roomNo)
        {
            return _roomServiceFood.GetRoomServiceFood(roomNo);
        }
        
        [HttpGet("{id}")] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<RoomService_Food> GetRoomServiceById(int id)
        {
            var roomServiceFood = _roomServiceFood.GetRoomServiceFoodById(id);
            if (roomServiceFood == null)
            {
                return NotFound();
            }

            return roomServiceFood;
        }
    }
}