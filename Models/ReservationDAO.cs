using System;
using System.Data;

namespace Hotel.Models
{
    public class ReservationDAO
    {
        public int Id { set; get; }
        public int CustomerId { set; get; }
        public int RoomNo { set; get; }
        public DateTime Date { set; get; }
        public DateTime? CheckInDate { set; get; }
        public DateTime? CheckOutDate { set; get; }
    }
}