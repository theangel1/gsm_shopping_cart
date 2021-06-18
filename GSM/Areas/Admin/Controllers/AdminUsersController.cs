using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GSM.Data;
using GSM.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GSM.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.ApplicationUser.ToList());
        }

        //GET : Details
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usersFromDB = await _db.ApplicationUser.SingleOrDefaultAsync(u => u.Id == id);

            if (usersFromDB == null)
                return NotFound();

            return View(usersFromDB);

        }
    }
}