namespace WEB_LAB9.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string ReservedBy { get; set; }
        public DateTime Time { get; set; }
        public string ReservationTime { get; set; }
        public DateTime ReservedDay { get; set; }
        public Room? Room { get; set; }
    }
}