namespace Hotel.Models
{
    public class BillDao
    {
        public int Id { set; get; }
        // public int ReservationId { set; get; }
        public int RoomServiceFoodId { set; get; }
        public float PriceOfRoom { set; get; }
        public float PriceOfRoomService { set; get; }
        public float TotalPrice { set; get; }
    }
}