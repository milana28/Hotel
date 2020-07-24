using System.Collections.Generic;

namespace Hotel.Models
{
    public class Bill
    {
        public int Id { set; get; }
        public Reservation Reservation { set; get; }
        public int RoomServiceFoodId { set; get; }
        public List<Food> Order { set; get; }
        public float PriceOfRoom { set; get; }
        public float PriceOfRoomService { set; get; }
        public float TotalPrice { set; get; }
    }
}