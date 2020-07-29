using System;
using System.Collections.Generic;

namespace Hotel.Models
{
    public class Bill
    {
        public DateTime Date { set; get; }
        public Reservation Reservation { set; get; }
        public List<Item> Order { set; get; }
        public float PriceOfRoom { set; get; }
        public float PriceOfRoomService { set; get; }
        public float TotalPrice { set; get; }
    }
}