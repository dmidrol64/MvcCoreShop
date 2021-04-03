using CoreStoreMVC.Data;
using CoreStoreMVC.Models;
using CoreStoreMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStoreMVC.Areas.Admin.Controllers
{
   [Authorize(Roles = SD.SuperAdminEndUser)]
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
            //if (id != null && id.Length > 0)
            //    id = id.Trim();

            if (id is null || id.Trim().Length is 0)
                return NotFound();


            var users = await _db.ApplicationUsers.FindAsync(id);


            if (users == null)
                return NotFound();

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>EditUsers(string id, ApplicationUser applicationUser)
        {
            if (applicationUser.Id != id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(applicationUser);

            var user = await _db.ApplicationUsers.FindAsync(id);
            if (user is null)
                return NotFound();

            user.Name = applicationUser.Name;
            user.PhoneNumber = applicationUser.PhoneNumber;
            
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Application user{applicationUser.Name} editing successful.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id is null || id.Trim().Length is 0)
                return NotFound();
            var user = await _db.ApplicationUsers.FindAsync(id);

            if (user is null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(string id)
        {
            var user = await _db.ApplicationUsers.FindAsync(id);

            user.LockoutEnd = DateTime.Now.AddYears(1000);

            await _db.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
    }
}
