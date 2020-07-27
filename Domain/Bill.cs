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
        List<Models.Bill> GetBills(int? reservationId);
        Models.Bill GetBillById(int id);
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
            var billDao = new BillDao()
            {
                Id = GenerateBillId(),
                RoomServiceFoodId = bill.RoomServiceFoodId,
                PriceOfRoom = bill.PriceOfRoom,
                PriceOfRoomService = bill.PriceOfRoomService,
                TotalPrice = bill.TotalPrice
            };
            
            if (CheckIfReservationExist(bill) == null)
            {
                return null;
            }
            
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string insertQuery =
                "INSERT INTO Hotel.Bill VALUES (@roomServiceFoodId, @priceOfRoom, @priceOfRoomService, @totalPrice)";
            database.Execute(insertQuery, billDao);
            
            return TransformDaoToBusinessLogicBill(billDao);
        }
        
        public List<Models.Bill> GetBills(int? reservationId)
        {
            return reservationId == null ? GetAll() : GetBillByReservationId(reservationId);
        }

        public Models.Bill GetBillById(int id)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string sql = "SELECT * FROM Hotel.Bill WHERE id = @billId";
            var billDao = database.QuerySingle<BillDao>(sql, new {billId = id});

            return TransformDaoToBusinessLogicBill(billDao);
        }
        
        private List<Models.Bill> GetAll()
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var billDao = database.Query<BillDao>("SELECT * FROM Hotel.Bill").ToList();
            var bills = new List<Models.Bill>();

            billDao.ForEach(b => bills.Add(TransformDaoToBusinessLogicBill(b)));

            return bills;
        }
        
        private List<Models.Bill> GetBillByReservationId(int? id)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var billDaoList = new List<BillDao>();
            var billList = new List<Models.Bill>();

            var roomServiceFoodList = _roomServiceFood.GetRoomServiceFoodByReservationId(id);
            
            roomServiceFoodList.ForEach(rf =>
            {
                using IDbConnection conn = new SqlConnection(DatabaseConnectionString);
                const string billQuery = "SELECT * FROM Hotel.Bill WHERE roomServiceFoodId = @roomServiceFoodId";
                billDaoList.Add(conn.QuerySingle<BillDao>(billQuery, new {roomServiceFoodId = rf.Id}));
            });
            
            billDaoList.ForEach(b => billList.Add(TransformDaoToBusinessLogicBill(b)));

            return billList;
        }
        
        private Models.Bill TransformDaoToBusinessLogicBill(BillDao billDao)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            
            const string roomServiceFoodQuery = "SELECT * FROM Hotel.RoomService_Food WHERE id = @id";
            var roomServiceFoodDao = database.QuerySingle<RoomServiceFoodDao>(roomServiceFoodQuery, new {id = billDao.RoomServiceFoodId});
            
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
        
        private Models.RoomServiceFood CheckIfRoomServiceFoodExist(int roomServiceFoodId, Models.Bill bill)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            
            var allRoomServiceFood = new List<Models.RoomServiceFood>();
            var roomServiceFood = _roomServiceFood.GetRoomServiceFoodById(roomServiceFoodId);
            
            var allRoomServiceFoodDao = database.Query<RoomServiceFoodDao>("SELECT * FROM Hotel.RoomService_Food").ToList();
            allRoomServiceFoodDao.ForEach(rs => allRoomServiceFood.Add(_roomServiceFood.TransformDaoToBusinessLogicRoomServiceFood(rs)));

            var roomServiceFoodList = allRoomServiceFood.Where(r =>
                                          r.Id == bill.RoomServiceFoodId && r.Food == bill.Order && r.RoomService.ReservationId == bill.Reservation.Id);
        
            return !roomServiceFoodList.Any() ? null : roomServiceFood;
        }
        
        private Models.Reservation CheckIfReservationExist(Models.Bill bill)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var reservations = database.Query<Models.Reservation>("SELECT * FROM Hotel.Reservation").ToList();

            var reservationsList = reservations.Where(r =>
                r.Id == bill.Reservation.Id && 
                r.Customer == bill.Reservation.Customer && 
                r.Room == bill.Reservation.Room && 
                r.Date.Equals(bill.Reservation.Date) && 
                r.CheckInDate.Equals(bill.Reservation.CheckInDate) && 
                r.CheckOutDate.Equals(bill.Reservation.CheckOutDate));
        
            return !reservationsList.Any() ? null : bill.Reservation;
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