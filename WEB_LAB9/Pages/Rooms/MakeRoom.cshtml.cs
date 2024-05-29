using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEB_LAB9.Data;
using WEB_LAB9.Models;

namespace WEB_LAB9.Pages.Rooms
{
    public class MakeModel : PageModel
    {
        private readonly AppDbContext _context;

        public MakeModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Room Room { get; set; }

        public IActionResult OnGet()
        {
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
    }
}
