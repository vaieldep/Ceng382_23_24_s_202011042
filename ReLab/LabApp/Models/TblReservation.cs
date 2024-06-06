using System;
using System.Collections.Generic;

namespace LabApp.Models;

public partial class TblReservation
{
    public int Id { get; set; }

    public int Time { get; set; }

    public int Date { get; set; }

    public int UserId { get; set; }

    public int RoomId { get; set; }
}
