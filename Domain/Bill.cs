using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Hotel.Models;

namespace Hotel.Domain
{
    public interface IBill
    {
        Models.Bill CreateBill(Models.Bill bill);
        List<Models.Bill> GetAll();
    }
    
    public class Bill : IBill
    {
        private const string DatabaseConnectionString = "Server=localhost;Database=hotel;User Id=sa;Password=yourStrong(!)Password;";
        private readonly IRoomServiceFood _roomServiceFood;
        private readonly IReservation _reservation;

        public Bill(IRoomServiceFood roomServiceFood, IReservation reservation)
        {
            _roomServiceFood = roomServiceFood;
            _reservation = reservation;
        }

        public Models.Bill CreateBill(Models.Bill bill)
        {
            var billDao = new BillDAO()
            {
                Id = GenerateBillId(),
                RoomServiceFoodId = bill.RoomServiceFoodId,
                PriceOfRoom = bill.PriceOfRoom,
                PriceOfRoomService = bill.PriceOfRoomService,
                TotalPrice = bill.TotalPrice
            };
            
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string insertQuery =
                "INSERT INTO Hotel.Bill VALUES (@roomServiceFoodId, @priceOfRoom, @priceOfRoomService, @totalPrice)";
            database.Execute(insertQuery, billDao);
            
            return TransformDaoToBusinessLogicBill(billDao);
        }

        public List<Models.Bill> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var billDao = database.Query<BillDAO>("SELECT * FROM Hotel.Bill").ToList();
            var bills = new List<Models.Bill>();

            billDao.ForEach(b => bills.Add(TransformDaoToBusinessLogicBill(b)));

            return bills;
        }
        
        private Models.Bill TransformDaoToBusinessLogicBill(BillDAO billDao)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            
            const string roomServiceFoodQuery = "SELECT * FROM Hotel.roomService_food WHERE id = @id";
            var roomServiceFoodDao = database.QuerySingle<RoomService_FoodDAO>(roomServiceFoodQuery, new {id = billDao.RoomServiceFoodId});
            
            var roomServiceFood = _roomServiceFood.TransformDaoToBusinessLogicRoomServiceFood(roomServiceFoodDao);
            var reservation = _reservation.GetReservationById(roomServiceFood.RoomService.ReservationId);
            var food = _roomServiceFood.GetFoodForRoomService(roomServiceFoodDao.RoomServiceId);
            
            return new Models.Bill()
            {
                Id = billDao.Id,
                Reservation = reservation,
                RoomServiceFoodId = billDao.RoomServiceFoodId,
                Order = food,
                PriceOfRoom = billDao.PriceOfRoom,
                PriceOfRoomService = billDao.PriceOfRoomService,
                TotalPrice = billDao.TotalPrice
            };
        }
        
        private int GenerateBillId()
        {
            var bills = GetAll();
            if (bills.Count == 0)
            {
                return 1;
            }
            var idList = new List<int>();
            bills.ForEach(r => idList.Add(r.Id));
            var last = idList.Max();

            return last + 1;
        }
    }
}