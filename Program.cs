using System;
using System.Collections.Generic;

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

    public class Room
    {
        public string Name { get; set; }
        public Dictionary<(int, DateTime), Reservation> Reservations { get; set; }

        public Room(string name)
        {
            Name = name;
            Reservations = new Dictionary<(int, DateTime), Reservation>();
        }
    }

    public class ReservationHandler

{

    private Dictionary<(string, int, DateTime), Reservation> _reservations;

    private Dictionary<string, Room> _rooms;


    private readonly string[] messages = { "Deniz", "Arda", "Kaan", "Emre", "Semih" };


    public ReservationHandler()

    {

        _reservations = new Dictionary<(string, int, DateTime), Reservation>();

        _rooms = new Dictionary<string, Room>();


        // Add sample rooms

        _rooms.Add("101", new Room("A"));

        _rooms.Add("102", new Room("B"));

        _rooms.Add("103", new Room("C"));

        _rooms.Add("104", new Room("D"));


        Random rand = new Random();


        for (int i = 0; i < 160; i++)

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

            }

        }

    }

        public void DisplayWeeklySchedule()
{
    Console.WriteLine("Enter the room ID:");
    string roomId = Console.ReadLine();

    if (!_rooms.ContainsKey(roomId))
    {
        Console.WriteLine("Error: Room not found.");
        return;
    }

    Console.WriteLine($"Weekly schedule for room {roomId}:");
    Console.WriteLine("|----------------|----------------------|----------------------|-----------------------|----------------------|----------------------|----------------------|------------------------|");
    Console.WriteLine("|      Time      |        Sunday        |        Monday        |        Tuesday        |       Wednesday      |       Thursday       |        Friday        |        Saturday        |");
    Console.WriteLine("|----------------|----------------------|----------------------|-----------------------|----------------------|----------------------|----------------------|------------------------|");

    for (int row = 9; row < 17; row++)
    {
        int startHour = row;
        int endHour = startHour + 1;

        if (startHour == 9)
        {
            Console.Write($"|  0{startHour}:00-{endHour}:00   |");

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
                Console.Write($"      {roomId}-{reservation.ReservedBy}        |");
            }
            else
            {
                Console.Write($"       {roomId}-Open       |");
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine("|----------------|----------------------|----------------------|-----------------------|----------------------|----------------------|----------------------|------------------------|");
}

        public void DisplayRoomSchedule(string roomId)
        {
            if (!_rooms.ContainsKey(roomId))
            {
                Console.WriteLine("Error: Room not found.");
                return;
            }

            Console.WriteLine($"Schedule for room {roomId}:");
            Console.WriteLine("|----------------|----------------------|----------------------|-----------------------|----------------------|----------------------|----------------------|------------------------|");
            Console.WriteLine("|      Time      |        Sunday        |        Monday        |        Tuesday        |       Wednesday      |       Thursday       |        Friday        |        Saturday        |");
            Console.WriteLine("|----------------|----------------------|----------------------|-----------------------|----------------------|----------------------|----------------------|------------------------|");

            for (int row = 9; row < 17; row++)
            {
                int startHour = row;
                int endHour = startHour + 1;

                if (startHour == 9)
                {
                    Console.Write($"|  0{startHour}:00-{endHour}:00   |");
                    
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
                        Console.Write($"      {roomId}-{reservation.ReservedBy}        |");
                    }
                    else
                    {
                        Console.Write($"       {roomId}-Open       |");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("|----------------|----------------------|----------------------|-----------------------|----------------------|----------------------|----------------------|------------------------|");

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

            Console.WriteLine($"Successfully reserved room {roomId} for {reservedBy} on {reservationTime}.");
            return true;
        }

        public bool CancelReservation(string roomId, int dayOfWeek, DateTime reservationTime)
        {
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

            Console.WriteLine($"Successfully canceled reservation for room {roomId} on {reservationTime}.");
            return true;
        }
    }
}
namespace ReservationSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ReservationHandler handler = new ReservationHandler();

            while (true)
            {
                Console.WriteLine("Enter '1' to make a reservation");
                Console.WriteLine("Enter '2' to cancel a reservation");
                Console.WriteLine("Enter '3' to view a room's schedule");
                Console.WriteLine("Enter '4' to view the weekly schedule");
                Console.WriteLine("Enter '5' to quit:");
                string command = Console.ReadLine()?.ToUpper();

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

                            Console.WriteLine("Enter the name of the person making the reservation:");
                            string reservedBy = Console.ReadLine();

                            handler.AddReservation(roomId, dayOfWeek, DateTime.Today.AddDays(dayOfWeek).AddHours(hour), reservedBy);
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
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;
                    case "4":
                        handler.DisplayWeeklySchedule();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid command.");
                        break;
                }
            }
        }
    }
}