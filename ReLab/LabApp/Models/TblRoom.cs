using System;
using System.Collections.Generic;

namespace LabApp.Models;

public partial class TblRoom
{
    public int Id { get; set; }

    public string? RoomName { get; set; }

    public int Capacity { get; set; }
}
