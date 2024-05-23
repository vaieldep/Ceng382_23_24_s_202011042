public class Reservation

{

public int roomId { get; set; }

public string ReservedBy { get; set; }

public DateTime Time { get; set; }


public DateTime ReservedDay { get; set; }

public Room ?Room { get; set; }

}