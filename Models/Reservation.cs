using System;

namespace Hotel.Models
{
    public class Reservation
    {
        public int Id { set; get; }
        public Customer Customer { set; get; }
        public Room Room { set; get; }
        public DateTime Date { set; get; }
        public DateTime CheckInDate { set; get; }
        public DateTime CheckOutDate { set; get; }
    }
}