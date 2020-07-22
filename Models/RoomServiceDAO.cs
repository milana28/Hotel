using System;

namespace Hotel.Models
{
    public class RoomServiceDAO
    {
        public int Id { set; get; }
        public int ReservationId { set; get; }
        public DateTime Date { set; get; }
    }
}