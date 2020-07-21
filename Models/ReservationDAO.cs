using System.Data;

namespace Hotel.Models
{
    public class ReservationDAO
    {
        public int Id { set; get; }
        public int CustomerId { set; get; }
        public int RoomNo { set; get; }
        public string CheckIn { set; get; }
        public string CheckOut { set; get; }
    }
}