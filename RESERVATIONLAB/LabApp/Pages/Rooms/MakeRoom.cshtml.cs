using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabApp.Data;
using LabApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Pages.Rooms
{
    public class MakeModel : PageModel
    {
        private readonly LabAppDataBaseContext _context;

        public MakeModel(LabAppDataBaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Room Room { get; set; }

        public IList<Room> Rooms { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Rooms = await _context.Rooms.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Rooms.Add(Room);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Oda eklendi";
            return RedirectToPage("/Rooms/MakeRoom");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Oda silindi";
            return RedirectToPage();
        }
    }
}
