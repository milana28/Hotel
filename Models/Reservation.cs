namespace Hotel.Models
{
    public class Reservation
    {
        public int Id { set; get; }
        public Customer Customer { set; get; }
        public Room Room { set; get; }
        public string CheckIn { set; get; }
        public string CheckOut { set; get; }
    }
}