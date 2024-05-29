using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WEB_LAB9.Data;
using WEB_LAB9.Models;

namespace WEB_LAB9.Pages.Rooms
{
    public class ShowRoomsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ShowRoomsModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Room> Rooms { get; set; }

        public async Task OnGetAsync()
        {
            Rooms = await _context.Rooms.ToListAsync();
        }
    }
}
