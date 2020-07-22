using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hotel.Models
{
    public class RoomService
    {
        public int Id { set; get; }
        public Reservation Reservation { set; get; }
        public DateTime Date { set; get; }
    }
}