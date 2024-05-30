using System;
using System.Collections.Generic;

namespace LabApp.Models;

public partial class Room
{
    public int Id { get; set; }

    public string RoomName { get; set; } = null!;

    public int Capacity { get; set; }

    public string? UserId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
