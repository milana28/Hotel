namespace Hotel.Models
{
    public class RoomDto
    {
        public int RoomNo { set; get; }
        public string Location { set; get; }
        public int PricePerDay { set; get; }
        public bool Available { set; get; }
    }
}