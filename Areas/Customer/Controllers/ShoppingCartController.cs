using CoreStoreMVC.Data;
using CoreStoreMVC.Extensions;
using CoreStoreMVC.Models;
using CoreStoreMVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStoreMVC.Areas.Customer.Controllers
{
    // Домашнее задание 9 *******************************************
    [Area(nameof(Customer))]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;

            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Product>()
            };
        }

        // GET Shopping Cart
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("sShoppingCart");

            if (shoppingCartList != null && shoppingCartList.Count > 0)
            {
                foreach (var cartItem in shoppingCartList)
                {
                    Product product = await _db.Products.Include(x => x.SpecialTags)
                                                  .Include(x => x.ProductTypes)
                                                  .Where(x => x.Id == cartItem)
                                                  .FirstOrDefaultAsync();

                    ShoppingCartVM.Products.Add(product);
                }
                
            }

            return View(ShoppingCartVM);
        }
        // **********************************************************************************************

        // Домашнее задание 10 *******************************************
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexPost()
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("sShoppingCart");

            ShoppingCartVM.Appointment.AppointmentDay = ShoppingCartVM.Appointment.AppointmentDay
                                                                      .AddHours(ShoppingCartVM.Appointment.AppointmentTime.Hour)
                                                                      .AddMinutes(ShoppingCartVM.Appointment.AppointmentTime.Minute);

            Appointment appointment = ShoppingCartVM.Appointment;

            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            int appointmentId = appointment.Id;

            foreach (var item in shoppingCartList)
            {
                ProductsForAppointment productsForAppointment = new ProductsForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = item
                };
                _db.ProductsForAppointments.Add(productsForAppointment);
            }
            await _db.SaveChangesAsync();

            shoppingCartList = new List<int>();
            HttpContext.Session.Set("sShoppingCart", shoppingCartList);

            // Lesson 12 2.2
            return RedirectToAction(nameof(AppointmentConfirmation), new { Id = appointmentId });
        }
        // **********************************************************************************************

        // Урок 12 *******************************************
        // 1
        public IActionResult Remove(int id)
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>("sShoppingCart");

            if (shoppingCartList.Count > 0)
            {
                if (shoppingCartList.Contains(id))
                    shoppingCartList.Remove(id);
            }

            HttpContext.Session.Set("sShoppingCart", shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        // 2.1
        [HttpGet]
        public async Task<IActionResult> AppointmentConfirmation(int id)
        {
            ShoppingCartVM.Appointment = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            List<ProductsForAppointment> productListObj = await _db.ProductsForAppointments.Where(x => x.AppointmentId == id).ToListAsync();

            foreach (var item in productListObj)
            {
                ShoppingCartVM.Products.Add(await _db.Products
                                                     .Include(x => x.ProductTypes)
                                                     .Include(x => x.SpecialTags)
                                                     .FirstOrDefaultAsync(x => x.Id == item.ProductId));
            }

            return View(ShoppingCartVM);
        }


        // **********************************************************************************************
    }
}
