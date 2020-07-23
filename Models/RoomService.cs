using System;


namespace Hotel.Models
{
    public class RoomService
    {
        public int Id { set; get; }
        public int ReservationId { set; get; }
        public Customer Customer { set; get; }
        public Room Room { set; get; }
        public DateTime Date { set; get; }
    }
}