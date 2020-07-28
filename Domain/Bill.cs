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
        private readonly IRoomService _roomService;
        private readonly IReservation _reservation;

        public Bill(IRoomServiceFood roomServiceFood, IReservation reservation, IRoomService roomService)
        {
            _roomServiceFood = roomServiceFood;
            _reservation = reservation;
            _roomService = roomService;
        }

        public Models.Bill CreateBill(Models.Bill bill)
        {
            var billDao = new BillDao()
            {
                Id = GenerateBillId(),
                RoomServiceId = bill.RoomServiceId,
                PriceOfRoom = bill.PriceOfRoom,
                PriceOfRoomService = bill.PriceOfRoomService,
                TotalPrice = bill.TotalPrice
            };
            
            // {
            //     "reservationId": 1,
            //     "roomServiceId": 1,
            //     "priceOfRoom": 30,
            //     "priceOfRoomService": 20,
            //     "totalPrice": 50
            // }
            
            if (CheckIfReservationExist(bill) == null || CheckIfRoomServiceFoodExist(bill.RoomServiceId, bill) == null)
            {
                return null;
            }
            
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            const string insertQuery =
                "INSERT INTO Hotel.Bill VALUES (@roomServiceId, @priceOfRoom, @priceOfRoomService, @totalPrice)";
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
            var roomService = _roomService.GetRoomServiceById(billDao.RoomServiceId);
            var reservation = _reservation.GetReservationById(roomService.ReservationId);
            var food = _roomServiceFood.GetFoodForRoomService(billDao.RoomServiceId);
            
            return new Models.Bill()
            {
                Id = billDao.Id,
                Reservation = reservation,
                RoomServiceId = billDao.RoomServiceId,
                Order = food,
                PriceOfRoom = billDao.PriceOfRoom,
                PriceOfRoomService = billDao.PriceOfRoomService,
                TotalPrice = billDao.TotalPrice
            };
        }
        
        private Models.RoomService CheckIfRoomServiceFoodExist(int roomServiceId, Models.Bill bill)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var allRoomServiceFood = new List<Models.RoomServiceFood>();
            var roomService = _roomService.GetRoomServiceById(roomServiceId);
            
            const string sql = "SELECT * FROM Hotel.RoomService_Food WHERE roomServiceid = @id";
            var allRoomServiceFoodDao = database.Query<RoomServiceFoodDao>(sql, new {id = roomServiceId}).ToList();
            
            allRoomServiceFoodDao.ForEach(rs => allRoomServiceFood.Add(_roomServiceFood.TransformDaoToBusinessLogicRoomServiceFood(rs)));
        
            var roomServiceFoodList = allRoomServiceFood.Where(r =>
                                          r.Id == bill.RoomServiceId && CheckIfFoodIsTheSame(r.RoomService.Id, bill.Order) && r.RoomService.ReservationId == bill.Reservation.Id);
        
            return !roomServiceFoodList.Any() ? null : roomService;
        }
        
        private Models.Reservation CheckIfReservationExist(Models.Bill bill)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var reservationsDao = database.Query<ReservationDao>("SELECT * FROM Hotel.Reservation").ToList();
            var reservations = new List<Models.Reservation>();
            reservationsDao.ForEach(el => reservations.Add(_reservation.TransformDaoToBusinessLogicReservation(el)));

            var reservationsList = reservations.Where(r =>
                r.Id == bill.Reservation.Id &&
                CheckIfCustomersAreTheSame(r.Customer,bill.Reservation.Customer) &&
                CheckIfRoomsAreTheSame(r.Room, bill.Reservation.Room) &&
                r.Date.Equals(bill.Reservation.Date) &&
                r.CheckInDate.Equals(bill.Reservation.CheckInDate) &&
                r.CheckOutDate.Equals(bill.Reservation.CheckOutDate));
            
            return !reservationsList.Any() ? null : bill.Reservation;
        }

        private static bool CheckIfCustomersAreTheSame(Models.Customer firstCustomer, Models.Customer secondCustomer)
        {
            return firstCustomer.Id == secondCustomer.Id && firstCustomer.Name == secondCustomer.Name &&
                   firstCustomer.Address == secondCustomer.Address && firstCustomer.PhoneNo == secondCustomer.PhoneNo;
        }

        private static bool CheckIfRoomsAreTheSame(Models.Room firstRoom, Models.Room secondRoom)
        {
            return firstRoom.RoomNo == secondRoom.RoomNo && firstRoom.Location == secondRoom.Location;
        }
        
        private bool CheckIfFoodIsTheSame(int roomServiceId, List<Models.Food> order)
        {
            using IDbConnection database = new SqlConnection(DatabaseConnectionString);
            var food = _roomServiceFood.GetFoodForRoomService(roomServiceId);
            var isTrue = new bool();
            
            food.ForEach(f =>
            {
                order.ForEach(sf =>
                {
                    if (f.Id == sf.Id && f.Name == sf.Name)
                    {
                        isTrue = true;
                    }
                    else
                    {
                        isTrue = false;
                    }
                });
            });

            return isTrue;
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