using System;
using System.Collections.Generic;

namespace Hotel.Models
{
    public class RoomServiceDto
    {
        public int Id { set; get; }
        public int ReservationId { set; get; } 
        public List<int> OrderId { set; get; }
    }
}