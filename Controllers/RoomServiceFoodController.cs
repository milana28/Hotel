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
        
        [HttpGet] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<RoomService_Food>> GetAll()
        {
            return _roomServiceFood.GetAll();
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