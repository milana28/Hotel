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
            var ordersForReservation = new List<Models.Item>();
            var priceOfRoomService = new float();
            var reservation = _reservation.GetReservationById(reservationId);

            orders.ForEach(order =>
            {
                order.Order.ForEach(el =>
                {
                    ordersForReservation.Add(el);
                    priceOfRoomService += el.Price;
                });
            });

            return new Models.Bill()
            {
                Date = DateTime.Now,
                Reservation = reservation,
                Order = ordersForReservation,
                PriceOfRoom = GetPriceOfRoom(reservation),
                PriceOfRoomService = priceOfRoomService,
                PriceWithoutPdv = GetPriceWithoutPdv(GetPriceOfRoom(reservation) + priceOfRoomService),
                PDV = 17,
                TotalPrice = GetPriceOfRoom(reservation) + priceOfRoomService
            };
        }
        
        public 

        private static double GetPriceWithoutPdv(float priceWithPdv)
        {
            return priceWithPdv - (0.17 * priceWithPdv);
        }

        private static long GetPriceOfRoom(Models.Reservation reservation)
        {
            var checkInDate = reservation.CheckInDate;
            var checkOutDate = reservation.CheckOutDate;
            var days = new int();
        
        
            if (checkInDate != null && checkOutDate != null)
            {
                days = ((TimeSpan) (checkOutDate - checkInDate)).Days; 
            }

            return  days * reservation.Room.PricePerDay;
        }
    }
}