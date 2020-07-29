using System;
using System.Collections.Generic;

namespace Hotel.Models
{
    public class RoomService
    {
        public int Id { set; get; }
        public int ReservationId { set; get; }
        public Customer Customer { set; get; }
        public Room Room { set; get; }
        public List<Item> Order { set; get; }
        public DateTime Date { set; get; }
    }
}