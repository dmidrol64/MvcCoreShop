using CoreStoreMVC.Data;
using CoreStoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStoreMVC.Areas.Admin.Controllers
{
   [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _db.ApplicationUsers.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsers(string id)
        {
            
            if (id.Length > 0)
                id = id.Trim();


            var users = await _db.ApplicationUsers.FindAsync(id);

            
            if (users.Id == null)
                return NotFound();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult>EditUser(string id, ApplicationUser applicationUser)
        {
            


            if(id.Length > 0)
                id = id.Trim();

            if (applicationUser.Id != id)
                return NotFound();

            _db.Update(applicationUser);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Application uer{applicationUser.Name} editing successful.";
            return RedirectToAction(nameof(Index));
        }
    }
}
