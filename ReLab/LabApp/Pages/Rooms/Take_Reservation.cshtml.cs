using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LabApp.Models;

namespace LabApp.Pages.Rooms
{
    public class WeeklyScheduleModel : PageModel
    {
        private readonly FinalDataBaseContext _context;

        public WeeklyScheduleModel(FinalDataBaseContext context)
        {
            _context = context;
        }

        public IList<TblRoom> Rooms { get; set; }
        public IList<TblReservation> Reservations { get; set; }
        [BindProperty]
        public TblReservation NewReservation { get; set; }

        public async Task OnGetAsync()
        {
            Rooms = await _context.TblRooms.ToListAsync();
            Reservations = await _context.TblReservations.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TblReservations.Add(NewReservation);
            await _context.SaveChangesAsync();
            System.IO.File.AppendAllText("Log.txt",$"\n{NewReservation.UserId}'li kullanıcı reservation oluşturdu.");
            TempData["SuccessMessage"] = "Reservation made successfully!";
            return RedirectToPage("./Take_Reservation");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var reservation = await _context.TblReservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            _context.TblReservations.Remove(reservation);
            System.IO.File.AppendAllText("Log.txt",$"\n{NewReservation.UserId}'li kullanıcı reservation sildi.");
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Reservation deleted successfully!";
            return RedirectToPage("./Take_Reservation");
        }
    }
}
