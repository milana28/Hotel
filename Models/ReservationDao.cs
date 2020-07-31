using System;

namespace Hotel.Models
{
    public class ReservationDao
    {
        public int Id { set; get; }
        public int CustomerId { set; get; }
        public int RoomNo { set; get; }
        public DateTime Date { set; get; }
        public DateTime PlannedArrivalDate { set; get; }
        public int DaysToStay { set; get; }
        public DateTime CheckInDate { set; get; }
        public DateTime? CheckOutDate { set; get; }
    }
}