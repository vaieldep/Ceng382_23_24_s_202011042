/*using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public List<string> WeeklySchedule { get; set; }

        public RoomData()
        {
            WeeklySchedule = new List<string>();
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
        public Dictionary<string, Room> _rooms;
        private List<LogData> _logData;
        public string userName = ""; // Define userName as a field of Program
        private string SaveRooms_filePath = @"C:\Users\deniz\Desktop\Ceng382\Ceng382_23_24_s_202011042\Data.json";
        private string LogData_filePath = @"C:\Users\deniz\Desktop\Ceng382\Ceng382_23_24_s_202011042\LogData.json";
        private string Reservation_filePath = @"C:\Users\deniz\Desktop\Ceng382\Ceng382_23_24_s_202011042\ReservationData.json";
        private readonly string[] messages = { "Deniz", "Arda", "Kaan", "Emre", "Semih", "Bartu", "Celal", "Berke" };

        public ReservationHandler()
        {
            _reservations = new Dictionary<(string, int, DateTime), Reservation>();
            _rooms = new Dictionary<string, Room>();
            _logData = new List<LogData>();

            _rooms.Add("101", new Room { Name = "A", Reservations = new List<Reservation>() });
            _rooms.Add("102", new Room { Name = "B", Reservations = new List<Reservation>() });
            _rooms.Add("103", new Room { Name = "C", Reservations = new List<Reservation>() });
            _rooms.Add("104", new Room { Name = "D", Reservations = new List<Reservation>() });

            foreach (var room in _rooms)
            {
                DisplayRoomWeeklySchedule(room.Key);
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
            int reservationCount;
            if (!int.TryParse(Console.ReadLine(), out reservationCount) || reservationCount <= 0)
            {
                Console.WriteLine("Error: Invalid input for reservation count.");
                return;
            }

            RandomlyFillReservations(reservationCount);
        }

        private void RandomlyFillReservations(int reservationCount)
        {
            Random rand = new Random();

            for (int i = 0; i < reservationCount; i++)
            {
                string roomId = _rooms.Keys.ElementAt(rand.Next(_rooms.Count));
                int dayOfWeek = rand.Next(7);
                int hour = rand.Next(9, 17);
                int randomIndex = rand.Next(messages.Length);
                string reservedBy = messages[randomIndex];
                DateTime reservationTime = DateTime.Today.AddDays(dayOfWeek).AddHours(hour);
                (string, int, DateTime) key = (roomId, dayOfWeek, reservationTime);
                if (!_reservations.ContainsKey(key))
                {
                    Reservation reservation = new Reservation(roomId, reservationTime, reservedBy);
                    _reservations.Add(key, reservation);

                    // Update room schedule
                    UpdateRoomSchedule(roomId, dayOfWeek, reservationTime);
                }
            }

            SaveRoomsToJson(_rooms); // Save the updated schedules to JSON
        }

        private void UpdateRoomSchedule(string roomId, int dayOfWeek, DateTime reservationTime)
        {
            if (!_rooms.ContainsKey(roomId))
            {
                Console.WriteLine("Error: Room not found.");
                return;
            }

            var room = _rooms[roomId];
            var scheduleEntry = $"{roomId}-{room.Reservations.Count + 1}";

            // Update weekly schedule
            for (int col = 0; col < 7; col++)
            {
                DateTime scheduleTime = DateTime.Today.AddDays(col).AddHours(reservationTime.Hour);

                if (scheduleTime.DayOfWeek == (DayOfWeek)dayOfWeek)
                {
                    room.Reservations.Add(new Reservation(roomId, scheduleTime, scheduleEntry));
                    break;
                }
            }
        }

        public bool CheckAdminPassword(string password)
        {
            // Replace "admin_password" with your actual admin password
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

            SaveRoomsToJson(_rooms); // Save the empty schedules to JSON
            SaveReservationsToJson(); // Save the empty reservations to JSON

            Console.WriteLine("All reservations and schedules have been reset.");
        }

        public void SaveRoomsToJson(Dictionary<string, Room> rooms)
        {
            List<RoomData> roomDataList = new List<RoomData>();

            foreach (var room in rooms.Values)
            {
                RoomData roomData = new RoomData
                {
                    RoomId = room.Name,
                    RoomName = room.Name,
                    Capacity = room.Reservations.Count,
                    WeeklySchedule = room.Reservations.Select(reservation => $"{reservation.ReservationTime.Hour}:00-{reservation.ReservationTime.Hour + 1}:00 - {reservation.ReservedBy}").ToList()
                };
                roomDataList.Add(roomData);
            }

            RoomsData roomsData = new RoomsData
            {
                Rooms = roomDataList
            };

            string jsonData = JsonConvert.SerializeObject(roomsData, Formatting.Indented);

            try
            {
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
            Console.WriteLine("|----------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|");
            Console.WriteLine("|      Time      |        Sunday        |        Monday        |        Tuesday       |       Wednesday      |       Thursday       |        Friday        |        Saturday      |");
            Console.WriteLine("|----------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|");

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
                    Console.Write($"|  {startHour}:00-{endHour}:00   |");
                }

                for (int col = 0; col < 7; col++)
                {
                    DateTime reservationTime = DateTime.Today.AddDays(col).AddHours(startHour);
                    Reservation? reservation = GetReservation(roomId, col, reservationTime);

                    if (reservation != null)
                    {
                        Console.Write($"       {roomId}-{reservation.ReservedBy}      |");
                    }
                    else
                    {
                        Console.Write($"        {roomId}-Open      |");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("|----------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|");

            UpdateRoomWeeklySchedule(roomId, roomData);
            SaveRoomsToJson(_rooms);
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

        public Reservation? GetReservation(string roomId, int dayOfWeek, DateTime reservationTime)
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

        public bool AddReservation(string roomId, int dayOfWeek, DateTime reservationTime, string reservedBy)
        {
            if (reservedBy != userName) // Access userName using Program.userName
            {
                Console.WriteLine("Error: You have no access to this reservation.");
                return false;
            }
            if (!_rooms.ContainsKey(roomId))
            {
                Console.WriteLine("Error: Room not found.");
                return false;
            }

            if (reservationTime.Hour < 9 || reservationTime.Hour >= 17)
            {
                Console.WriteLine("Error: Invalid reservation time.");
                return false;
            }

            (string, int, DateTime) key = (roomId, dayOfWeek, reservationTime);

            if (_reservations.ContainsKey(key))
            {
                Console.WriteLine("Error: Room is already reserved at this time.");
                return false;
            }

            Reservation reservation = new Reservation(roomId, reservationTime, reservedBy);
            _reservations.Add(key, reservation);

            LogData logEntry = new LogData($"Reserved room {roomId} at {reservationTime} by {reservedBy}", DateTime.Now, reservedBy);
            _logData.Add(logEntry);
            SaveLogsToJson(); // Save logs after adding a reservation

            // Call UpdateRoomSchedule to update the room's schedule after adding the reservation
            UpdateRoomSchedule(roomId, dayOfWeek, reservationTime);

            SaveRoomsToJson(_rooms); // Save the updated schedules to JSON

            Console.WriteLine($"Successfully reserved room {roomId} for {reservedBy} on {reservationTime}.");
            return true; // Return true if reservation is added successfully
        }

        public bool CancelReservation(string roomId, int dayOfWeek, DateTime reservationTime)
        {
            string reservedBy = _rooms[roomId].Reservations.Find(r => r.ReservationTime == reservationTime)?.ReservedBy;

            if (reservedBy != userName)
            {
                Console.WriteLine("Error: You have no access to this reservation.");
                return false;
            }
            if (!_rooms.ContainsKey(roomId))
            {
                Console.WriteLine("Error: Room not found.");
                return false;
            }

            if (reservationTime.Hour < 9 || reservationTime.Hour >= 17)
            {
                Console.WriteLine("Error: Invalid reservation time.");
                return false;
            }

            (string, int, DateTime) key = (roomId, dayOfWeek, reservationTime);

            if (!_reservations.ContainsKey(key))
            {
                Console.WriteLine("Error: No reservation found at this time.");
                return false;
            }

            _reservations.Remove(key);

            // Log the cancellation
            string reservedByLog = _reservations[key].ReservedBy; // Get the name of the person who reserved the room
            LogData logEntry = new LogData($"Canceled reservation for room {roomId} at {reservationTime} by {reservedByLog}", DateTime.Now, reservedByLog);
            _logData.Add(logEntry);
            SaveLogsToJson(); // Save logs after canceling a reservation

            // Call SaveLogsToJson after adding the log entry
            SaveLogsToJson();

            // Update ReservationData JSON file
            SaveReservationsToJson();

            Console.WriteLine($"Successfully canceled reservation for room {roomId} on {reservationTime}.");
            return true;
        }

        public void SaveLogsToJson()
        {
            string jsonData = JsonConvert.SerializeObject(_logData, Formatting.Indented);

            try
            {
                File.WriteAllText(LogData_filePath, jsonData);
                Console.WriteLine("Log data saved to " + LogData_filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving log data: " + ex.Message);
            }
        }

        public void SaveReservationsToJson()
        {
            List<Reservation> reservations = new List<Reservation>(_reservations.Values);
            string jsonData = JsonConvert.SerializeObject(reservations, Formatting.Indented);

            try
            {
                File.WriteAllText(Reservation_filePath, jsonData);
                Console.WriteLine("Reservation data saved to " + Reservation_filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving reservation data: " + ex.Message);
            }
        }

        public void DisplayRoomSchedule(string roomId)
        {
            if (!_rooms.ContainsKey(roomId))
            {
                Console.WriteLine("Error: Room not found.");
                return;
            }

            Console.WriteLine($"Schedule for room {roomId}:");
            Console.WriteLine("Weekly schedule for all rooms:");
            Console.WriteLine("|----------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|");
            Console.WriteLine("|      Time      |        Sunday        |        Monday        |        Tuesday       |       Wednesday      |       Thursday       |        Friday        |        Saturday      |");
            Console.WriteLine("|----------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|");

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

                    if (reservation != null)
                    {
                        Console.Write($"      {roomId}-{reservation.ReservedBy.PadRight(15)}        |");
                    }
                    else
                    {
                        Console.Write($"       {roomId}-Open        |");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("|----------------|----------------------|----------------------|-----------------------|----------------------|----------------------|----------------------|------------------------|");
        }
        public void DisplayWeeklySchedule()
        {
            Console.WriteLine("Weekly schedule for all rooms:");
            Console.WriteLine("|----------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|");
            Console.WriteLine("|      Time      |        Sunday        |        Monday        |        Tuesday       |       Wednesday      |       Thursday       |        Friday        |        Saturday      |");
            Console.WriteLine("|----------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|");


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
                    bool reservationFound = false;
                    foreach (var room in _rooms)
                    {
                        DateTime reservationTime = DateTime.Today.AddDays(col).AddHours(startHour);
                        Reservation? reservation = GetReservation(room.Key, col, reservationTime);

                        if (reservation != null)
                        {
                            Console.Write($"      {room.Key}-{reservation.ReservedBy.PadRight(15)}|");
                            reservationFound = true;
                            break;
                        }
                    }

                    if (!reservationFound)
                    {
                        Console.Write($"      Open      |");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("|----------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|----------------------|");
        }

    }


    public class Program
    {
        public string userName { get; set; }

        public static void Main(string[] args)
        {
            ReservationHandler handler = new ReservationHandler();

            Console.WriteLine("Enter your name:");
            handler.userName = Console.ReadLine();

            while (true)
            {
                Console.WriteLine("Enter '1' to make a reservation");
                Console.WriteLine("Enter '2' to cancel a reservation");
                Console.WriteLine("Enter '3' to view a room's schedule");
                Console.WriteLine("Enter '4' to view the weekly schedule");
                Console.WriteLine("Enter '5' to reset all reservations (need admin verification)");
                Console.WriteLine("Enter '6' to randomly fill reservations (need admin verification)");
                Console.WriteLine("Enter '7' to quit");
                string command = Console.ReadLine();

                switch (command)
                {
                    case "1":
                        try
                        {
                            Console.WriteLine("Enter the room ID:");
                            string roomId = Console.ReadLine();

                            Console.Write("Enter day of week (0-6): ");
                            int dayOfWeek = int.Parse(Console.ReadLine());

                            Console.Write("Enter hour (9-16): ");
                            int hour = int.Parse(Console.ReadLine());

                            handler.AddReservation(roomId, dayOfWeek, DateTime.Today.AddDays(dayOfWeek).AddHours(hour), handler.userName);
                            handler.LogUserInteraction("User made a reservation.", handler.userName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;
                    case "2":
                        try
                        {
                            Console.WriteLine("Enter the room ID:");
                            string roomId = Console.ReadLine();

                            Console.Write("Enter day of week (0-6): ");
                            int dayOfWeek = int.Parse(Console.ReadLine());

                            Console.Write("Enter hour (9-16): ");
                            int hour = int.Parse(Console.ReadLine());

                            handler.CancelReservation(roomId, dayOfWeek, DateTime.Today.AddDays(dayOfWeek).AddHours(hour));
                            handler.LogUserInteraction("User canceled a reservation.", handler.userName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;
                    case "3":
                        try
                        {
                            Console.WriteLine("Enter the room ID:");
                            string roomId = Console.ReadLine();

                            handler.DisplayRoomSchedule(roomId);
                            handler.LogUserInteraction("User viewed a room schedule.", handler.userName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;
                    case "4":
                        try
                        {
                            handler.DisplayWeeklySchedule();
                            handler.LogUserInteraction("User viewed the weekly schedule.", handler.userName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;
                    case "5":
                        try
                        {
                            Console.WriteLine("Enter admin password:");
                            string adminPassword = Console.ReadLine();

                            handler.ResetAllReservationsAndSchedules(adminPassword);
                            handler.LogUserInteraction("Admin reset all reservations and schedules.", "Admin");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;
                    case "6":
                        try
                        {
                            Console.WriteLine("Enter admin password:");
                            string adminPassword = Console.ReadLine();
                            handler.StartAutomaticReservation(adminPassword);
                            handler.LogUserInteraction("Admin started automatic reservations.", "Admin");
                            Console.WriteLine("Automatic reservation started successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("Invalid command. Please try again.");
                        break;
                }
            }
        }   

   }
}
*/