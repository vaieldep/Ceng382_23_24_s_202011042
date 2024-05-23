using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace ReservationSystem
{
    public class Reservation
    {
        public string RoomId { get; set; }
        public DateTime ReservationTime { get; set; }
        public string ReservedBy { get; set; }

        public Reservation(string roomId, DateTime reservationTime, string reservedBy)
        {
            RoomId = roomId;
            ReservationTime = reservationTime;
            ReservedBy = reservedBy;
        }
    }

    public class RoomData
    {
        public string RoomId { get; set; }
        public string ReservedBy { get; set; }
        public DateTime ReservationTime { get; set; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public List<string> WeeklySchedule { get; set; }

        public RoomData()
        {
            WeeklySchedule = new List<string>(new string[63]);
        }
    }

    public class RoomsData
    {
        public List<RoomData> Rooms { get; set; }
    }

    public class Room
    {
        public string Name { get; set; }
        public List<Reservation> Reservations { get; set; }
    }

    public class LogData
    {
        public string LogMessage { get; set; }
        public DateTime LogTime { get; set; }
        public string ReservedBy { get; set; }

        public LogData(string logMessage, DateTime logTime, string reservedBy)
        {
            LogMessage = logMessage;
            LogTime = logTime;
            ReservedBy = reservedBy;
        }
    }

    public class ReservationHandler
    {
        private Dictionary<(string, int, DateTime), Reservation> _reservations;
        private Reservation? GetReservation(string roomId, int dayOfWeek, DateTime reservationTime)
        {
            (string, int, DateTime) key = (roomId, dayOfWeek, reservationTime);

            if (_reservations.ContainsKey(key))
            {
                return _reservations[key];
            }
            else
            {
                return null;
            }
        }
        public Dictionary<string, Room> _rooms;
        private List<LogData> _logData;
        public string Random_reservedBy;
        public string userName;
        private List<RoomData> _roomDataList;
        private string SaveRooms_filePath = @"C:\Users\deniz\Desktop\Ceng382\Ceng382_23_24_s_202011042\Data.json";
        private string LogData_filePath = @"C:\Users\deniz\Desktop\Ceng382\Ceng382_23_24_s_202011042\LogData.json";
        private string Reservation_filePath = @"C:\Users\deniz\Desktop\Ceng382\Ceng382_23_24_s_202011042\ReservationData.json";
        private readonly string[] messages = { "Deniz", "Arda", "Kaan", "Emre", "Semih", "Bartu", "Celal", "Berke" };

        public ReservationHandler()
        {
            _reservations = new Dictionary<(string, int, DateTime), Reservation>();
            _roomDataList = new List<RoomData>();
            _rooms = new Dictionary<string, Room>();
            _logData = new List<LogData>();

            _rooms.Add("101", new Room { Name = "A", Reservations = new List<Reservation>() });
            _rooms.Add("102", new Room { Name = "B", Reservations = new List<Reservation>() });
            _rooms.Add("103", new Room { Name = "C", Reservations = new List<Reservation>() });
            _rooms.Add("104", new Room { Name = "D", Reservations = new List<Reservation>() });
            
            LoadRoomsFromJson();
            foreach (var room in _rooms)
            {
                DisplayRoomWeeklySchedule(room.Key);
            }
            
        }

        private void LoadRoomsFromJson()
        {
            try
            {
                string jsonData = File.ReadAllText(SaveRooms_filePath);
                RoomsData? roomsData = JsonConvert.DeserializeObject<RoomsData>(jsonData);
                if (roomsData != null)
                {
                    foreach (var roomData in roomsData.Rooms)
                    {
                    _roomDataList.Add(roomData);
                    }

                }
                Console.WriteLine("Rooms data loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading rooms data: " + ex.Message);
            }
        }

        public void LogUserInteraction(string logMessage, string reservedBy)
        {
            LogData logEntry = new LogData(logMessage, DateTime.Now, reservedBy);
            _logData.Add(logEntry);
            SaveLogsToJson();
        }

        public void StartAutomaticReservation(string adminPassword)
        {
            if (!CheckAdminPassword(adminPassword))
            {
                Console.WriteLine("Error: Incorrect admin password. Access denied.");
                return;
            }

            Console.WriteLine("Enter the number of random fillings:");
            if (!int.TryParse(Console.ReadLine(), out int reservationCount) || reservationCount <= 0)
            {
                Console.WriteLine("Error: Invalid input for reservation count.");
                return;
            }

            RandomlyFillReservations(reservationCount);
        }

        public void RandomlyFillReservations(int reservationCount)
        {
            Random rand = new Random();

            for (int i = 0; i < reservationCount; i++)
            {
                string roomId = _rooms.Keys.ElementAt(rand.Next(_rooms.Count));
                int dayOfWeek = rand.Next(0, 7); // Only weekdays
                int hour = rand.Next(9, 17);
                string Random_reservedBy = messages[rand.Next(messages.Length)];
                DateTime reservationTime = DateTime.Today.AddDays(dayOfWeek).AddHours(hour);
                (string, int, DateTime) key = (roomId, dayOfWeek, reservationTime);
                if (!_reservations.ContainsKey(key))
                {
                    Reservation reservation = new Reservation(roomId, reservationTime, Random_reservedBy);
                    _reservations.Add(key, reservation);

                    UpdateRoomSchedule_For_Random(roomId, dayOfWeek, reservationTime);
                }
            }

            SaveRoomsToJson();
            SaveReservationsToJson();
        }


        public void UpdateRoomSchedule_For_Random(string roomId, int dayOfWeek, DateTime reservationTime)
        {
            var roomData = _roomDataList.FirstOrDefault(r => r.RoomId == roomId);

            if (roomData != null)
            {
                if (reservationTime.Hour >= 9 && reservationTime.Hour < 17)
                {
                    if (dayOfWeek >= 0 && dayOfWeek <= 6)
                    {
                        int index = dayOfWeek * 9 + (reservationTime.Hour - 9);
                        if (index >= 0 && index < roomData.WeeklySchedule.Count)
                        {
                            string reservationInfo = $"{reservationTime.Hour:00}-{reservationTime.Hour + 1:00} {Random_reservedBy}";
                            roomData.WeeklySchedule[index] = reservationInfo;
                            Console.WriteLine($"Reservation added for room {roomId} on {reservationTime}.");
                        }
                        else
                        {
                            Console.WriteLine("Error: Invalid day of week or reservation time.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Invalid day of week.");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Invalid reservation time.");
                }
            }
            else
            {
                Console.WriteLine("Error: Room not found.");
            }
            SaveRoomsToJson();
        }
        public void UpdateRoomSchedule(string roomId, int dayOfWeek, DateTime reservationTime)
        {
            var roomData = _roomDataList.FirstOrDefault(r => r.RoomId == roomId);

            if (roomData != null)
            {
                if (reservationTime.Hour >= 9 && reservationTime.Hour < 17)
                {
                    if (dayOfWeek >= 0 && dayOfWeek <= 6)
                    {
                        int index = dayOfWeek * 9 + (reservationTime.Hour - 9);
                        if (index >= 0 && index < roomData.WeeklySchedule.Count)
                        {
                            string reservationInfo = $"{reservationTime.Hour:00}-{reservationTime.Hour + 1:00} {userName}";
                            roomData.WeeklySchedule[index] = reservationInfo;
                            Console.WriteLine($"Reservation added for room {roomId} on {reservationTime}.");
                        }
                        else
                        {
                            Console.WriteLine("Error: Invalid day of week or reservation time.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Invalid day of week.");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Invalid reservation time.");
                }
            }
            else
            {
                Console.WriteLine("Error: Room not found.");
            }
            SaveRoomsToJson();
        }

        public bool CheckAdminPassword(string password)
        {
            return password == "435918";
        }

        public void ResetAllReservationsAndSchedules(string password)
        {
            if (!CheckAdminPassword(password))
            {
                Console.WriteLine("Error: Incorrect password. Access denied.");
                return;
            }

            _reservations.Clear();
            foreach (var room in _rooms.Values)
            {
                room.Reservations.Clear();
            }

            SaveRoomsToJson();
            SaveReservationsToJson();

            Console.WriteLine("All reservations and schedules have been reset.");
        }

        private string SerializeRoomsDictionary(Dictionary<string, Room> rooms)

        {

            List<RoomData> roomDataList = new List<RoomData>();

            foreach (var room in rooms)

            {

                RoomData roomData = new RoomData

                {

                    RoomId = room.Key,

                    RoomName = room.Value.Name,

                    Capacity = room.Value.Reservations.Count,

                    WeeklySchedule = new List<string>() // Initialize a new list here

                };


                for (int row = 9; row < 17; row++)

                {

                    int startHour = row;

                    int endHour = startHour + 1;


                    string scheduleEntry = $"{startHour}:00-{endHour}:00";

                    for (int col = 0; col < 7; col++)

                    {

                        DateTime reservationTime = DateTime.Today.AddDays(col).AddHours(startHour);

                        Reservation? reservation = GetReservation(room.Key, col, reservationTime);


                        if (reservation != null)

                        {

                            scheduleEntry += $"      {room.Key}-{reservation.ReservedBy}";

                        }

                        else

                        {

                            scheduleEntry += $"       {room.Key}-Open";

                        }

                    }

                    roomData.WeeklySchedule.Add(scheduleEntry);

                }

                roomDataList.Add(roomData);

            }

            return JsonConvert.SerializeObject(new RoomsData { Rooms = roomDataList }, Formatting.Indented);

        }

        public void SaveRoomsToJson()
        {
            try
            {
                RoomsData roomsData = new RoomsData
                {
                    Rooms = _roomDataList
                };

                string jsonData = JsonConvert.SerializeObject(roomsData, Formatting.Indented);
                File.WriteAllText(SaveRooms_filePath, jsonData);
                Console.WriteLine("Rooms data saved to " + SaveRooms_filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }

        public void DisplayRoomWeeklySchedule(string roomId)
{
    if (!_rooms.ContainsKey(roomId))
    {
        Console.WriteLine("Error: Room not found.");
        return;
    }

    var room = _rooms[roomId];
    var roomData = new RoomData
    {
        RoomId = roomId,
        RoomName = room.Name,
        Capacity = room.Reservations.Count,
        WeeklySchedule = new List<string>() // Initialize a new list here
    };

    Console.WriteLine($"Weekly schedule for room {roomId}:");
    Console.WriteLine("|---------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|");
    Console.WriteLine("|        Time         |        Sunday        |        Monday        |        Tuesday       |       Wednesday      |       Thursday       |        Friday        |        Saturday      |");
    Console.WriteLine("|---------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|");

    for (int row = 9; row < 17; row++)
    {
        int startHour = row;
        int endHour = startHour + 1;

        if (startHour == 9)
        {
            Console.Write($"|     0{startHour}:00-{endHour}:00     |");
        }
        else
        {
            Console.Write($"|     {startHour}:00-{endHour}:00     |");
        }

        for (int col = 0; col < 7; col++)
        {
            DateTime reservationTime = DateTime.Today.AddDays(col).AddHours(startHour);
            Reservation? reservation = GetReservation(roomId, col, reservationTime);

            string reservationInfo = reservation != null ? $"{roomId}-{reservation.ReservedBy}" : "  Open  ";
            Console.Write($"       {reservationInfo.PadRight(15)}|");
        }
        Console.WriteLine();
    }
    Console.WriteLine("|---------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|");

    UpdateRoomWeeklySchedule(roomId, roomData);
    SaveRoomsToJson(); // Call SaveRoomsToJson without any arguments
}
private void UpdateRoomWeeklySchedule(string roomId, RoomData roomData)
{
    roomData.WeeklySchedule.Clear();
    for (int row = 9; row < 17; row++)
    {
        int startHour = row;
        int endHour = startHour + 1;

        string scheduleEntry = $"{startHour}:00-{endHour}:00";
        for (int col = 0; col < 7; col++)
        {
            DateTime reservationTime = DateTime.Today.AddDays(col).AddHours(startHour);
            Reservation? reservation = GetReservation(roomId, col, reservationTime);

            if (reservation != null)
            {
                scheduleEntry += $"      {roomId}-{reservation.ReservedBy}";
            }
            else
            {
                scheduleEntry += $"       {roomId}-Open";
            }
        }
        roomData.WeeklySchedule.Add(scheduleEntry);
    }
}

        public void SaveReservationsToJson()
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(_reservations.Values.ToList(), Formatting.Indented);
                File.WriteAllText(Reservation_filePath, jsonData);
                Console.WriteLine("Reservations saved to " + Reservation_filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving reservations data: " + ex.Message);
            }
        }

        public void SaveLogsToJson()
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(_logData, Formatting.Indented);
                File.WriteAllText(LogData_filePath, jsonData);
                Console.WriteLine("Logs saved to " + LogData_filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving logs data: " + ex.Message);
            }
        }

        public void AddReservation(string roomId, int dayOfWeek, DateTime reservationTime, string reservedBy)
        {
            if (!_rooms.ContainsKey(roomId))
            {
                Console.WriteLine("Error: Room not found.");
                return;
            }

            if (reservationTime.Hour < 9 || reservationTime.Hour >= 17)
            {
                Console.WriteLine("Error: Invalid reservation time. Please choose a time between 9 AM and 5 PM.");
                return;
            }

            (string, int, DateTime) key = (roomId, dayOfWeek, reservationTime);
            if (_reservations.ContainsKey(key))
            {
                Console.WriteLine("Error: Time slot already reserved.");
                return;
            }

            Reservation reservation = new Reservation(roomId, reservationTime, reservedBy);
            _reservations[key] = reservation;
            _rooms[roomId].Reservations.Add(reservation);

            UpdateRoomSchedule(roomId, dayOfWeek, reservationTime);
            SaveRoomsToJson();
            SaveReservationsToJson();
            LogUserInteraction($"Reservation added for room {roomId} on {reservationTime}.", reservedBy);
        }

        public void CancelReservation(string roomId, int dayOfWeek, DateTime reservationTime, string reservedBy)
        {
            if (!_rooms.ContainsKey(roomId))
            {
                Console.WriteLine("Error: Room not found.");
                return;
            }

            (string, int, DateTime) key = (roomId, dayOfWeek, reservationTime);
            if (!_reservations.ContainsKey(key))
            {
                Console.WriteLine("Error: Reservation not found.");
                return;
            }

            Reservation reservation = _reservations[key];
            if (reservation.ReservedBy != reservedBy)
            {
                Console.WriteLine("Error: You can only cancel your own reservations.");
                return;
            }

            _reservations.Remove(key);
            _rooms[roomId].Reservations.Remove(reservation);

            var roomData = _roomDataList.FirstOrDefault(r => r.RoomId == roomId);
            if (roomData != null)
            {
                int index = dayOfWeek * 9 + (reservationTime.Hour - 9);
                if (index >= 0 && index < roomData.WeeklySchedule.Count)
                {
                    roomData.WeeklySchedule[index] = null; // Clear the reservation slot
                }
            }
            
            SaveRoomsToJson();
            SaveReservationsToJson();
            LogUserInteraction($"Reservation canceled for room {roomId} on {reservationTime}.", reservedBy);
        }

        public List<Reservation> GetReservations()
        {
            return _reservations.Values.ToList();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ReservationHandler reservationHandler = new ReservationHandler();

            Console.Write("Enter your name: ");
            string userName = Console.ReadLine();
            reservationHandler.userName = userName;

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Add Reservation");
                Console.WriteLine("2. Cancel Reservation");
                Console.WriteLine("3. Display Reservations");
                Console.WriteLine("4. view a room's schedule");
                Console.WriteLine("5. Start Automatic Reservation");
                Console.WriteLine("6. Reset All Reservations and Schedules");
                Console.WriteLine("7. Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Room ID: ");
                        string roomId = Console.ReadLine();
                        Console.Write("Enter Day of Week (0-6, where 0 is Sunday and 6 is Saturday): ");
                        int dayOfWeek = int.Parse(Console.ReadLine());
                        Console.Write("Enter Reservation Hour (9-16): ");
                        int reservationHour = int.Parse(Console.ReadLine());
                        DateTime reservationTime = DateTime.Today.AddDays(dayOfWeek).AddHours(reservationHour);

                        reservationHandler.AddReservation(roomId, dayOfWeek, reservationTime, userName);
                        break;

                    case "2":
                        Console.Write("Enter Room ID: ");
                        roomId = Console.ReadLine();
                        Console.Write("Enter Day of Week (0-6, where 0 is Sunday and 6 is Saturday): ");
                        dayOfWeek = int.Parse(Console.ReadLine());
                        Console.Write("Enter Reservation Hour (9-16): ");
                        reservationHour = int.Parse(Console.ReadLine());
                        reservationTime = DateTime.Today.AddDays(dayOfWeek).AddHours(reservationHour);

                        reservationHandler.CancelReservation(roomId, dayOfWeek, reservationTime, userName);
                        break;

                    case "3":
                        List<Reservation> reservations = reservationHandler.GetReservations();
                        foreach (var reservation in reservations)
                        {
                            Console.WriteLine($"Room ID: {reservation.RoomId}, Reservation Time: {reservation.ReservationTime}, Reserved By: {reservation.ReservedBy}");
                        }
                        reservationHandler.LogUserInteraction("User viewed reservations.",userName);
                        break;
                    
                    case "4":
                        try
                        {
                            Console.WriteLine("Enter the room ID:");
                            string roomIdTemp = Console.ReadLine();

                            reservationHandler.DisplayRoomWeeklySchedule(roomIdTemp);
                            reservationHandler.LogUserInteraction("User viewed a room schedule.",userName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;

                    case "5":
                        Console.Write("Enter Admin Password: ");
                        string adminPassword = Console.ReadLine();
                        reservationHandler.StartAutomaticReservation(adminPassword);
                        reservationHandler.LogUserInteraction("Admin started automatic reservation.","Admin");
                        break;

                    case "6":
                        Console.Write("Enter Admin Password: ");
                        adminPassword = Console.ReadLine();
                        reservationHandler.ResetAllReservationsAndSchedules(adminPassword);
                        reservationHandler.LogUserInteraction("Admin deleted all the reservations.","Admin");
                        break;

                    case "7":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
