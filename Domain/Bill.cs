using System;
using System.Collections.Generic;

namespace Hotel.Domain
{
    public interface IBill
    {
        Models.Bill GenerateBill(int reservationId);
    }
    
    public class Bill : IBill
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        private readonly IRoomService _roomService;
        private readonly IReservation _reservation;

        public Bill(IReservation reservation, IRoomService roomService)
        {
            _reservation = reservation;
            _roomService = roomService;
        }

        public Models.Bill GenerateBill(int reservationId)
        { 
            var orders = _roomService.GetRoomServiceByReservationId(reservationId);
            var ordersForReservation = new List<Models.Food>();
            
            orders.ForEach(order =>
            {
                ordersForReservation = order.Order;
            });

            return new Models.Bill()
            {
                Date = DateTime.Now,
                Reservation = _reservation.GetReservationById(reservationId),
                Order = ordersForReservation,
                PriceOfRoom = 0,
                PriceOfRoomService = 0,
                TotalPrice = 0
            };
        }
    }
}