using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Pages.Rooms
{
    public class SeeScheduleModel : PageModel
    {
        private readonly FinalDataBaseContext _context;

        public SeeScheduleModel(FinalDataBaseContext context)
        {
            _context = context;
        }

        public IList<TblRoom> Rooms { get; set; }
        public IList<TblReservation> Reservations { get; set; }

        public async Task OnGetAsync()
        {
            Rooms = await _context.TblRooms.ToListAsync();
            Reservations = await _context.TblReservations.ToListAsync();
        }
    }
}
