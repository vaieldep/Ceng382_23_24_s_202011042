using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Pages.Rooms
{
    public class MakeModel : PageModel
    {
        private readonly FinalDataBaseContext _context;

        public MakeModel(FinalDataBaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TblRoom Room { get; set; }

        public IList<TblRoom> Rooms { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Rooms = await _context.TblRooms.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TblRooms.Add(Room);
            await _context.SaveChangesAsync();
            System.IO.File.AppendAllText("Log.txt",$"\n{Room.RoomName} isimli oda oluşturdu.");
            TempData["SuccessMessage"] = "Oda eklendi";
            return RedirectToPage("/Rooms/MakeRoom");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var room = await _context.TblRooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.TblRooms.Remove(room);
            await _context.SaveChangesAsync();
            System.IO.File.AppendAllText("Log.txt",$"\n{Room.RoomName} isimli oda silindi.");
            TempData["SuccessMessage"] = "Oda silindi";
            return RedirectToPage();
        }
    }
}
