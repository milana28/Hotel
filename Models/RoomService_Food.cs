using System.Collections.Generic;

namespace Hotel.Models
{
    public class RoomService_Food
    {
        public int Id { set; get; }
        public RoomService RoomService { set; get; }
        public List<Food> Food { set; get; }
    }
}