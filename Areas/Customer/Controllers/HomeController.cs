using Microsoft.AspNetCore.Mvc;
using CoreStoreMVC.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using CoreStoreMVC.Extensions;

namespace CoreStoreMVC.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        // Урок 8
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Получаем и записываем в лист все товары
            var productList = await _db.Products.Include(x => x.ProductTypes)
                                                .Include(x => x.SpecialTags)
                                                .ToListAsync();

            // 2. Возвращаем лист в представление
            return View(productList);
        }

        // GET: Index/Details/Id
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products.Include(x => x.ProductTypes)
                                            .Include(x => x.SpecialTags)
                                            .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
                return NotFound();

            return View(product);
        }

        // Урок 9
        // POST: Index/Details/Id
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPost(int id)
        {
            // Создать массив чисел и записываем в него десериализованные данные из сессии
            List<int> listOfShoppingCart = HttpContext.Session.Get<List<int>>("sShoppingCart");

            // Проверяем, если массив равен null, то создаём новый экземпляр массива
            if (listOfShoppingCart is null)
                listOfShoppingCart = new List<int>();

            // Добавляем полученный ID в массив
            listOfShoppingCart.Add(id);

            // Сериализуем и записываем в сессию массив с ID товаров
            HttpContext.Session.Set("sShoppingCart", listOfShoppingCart);

            // Переадресовываем на метод Index
            return RedirectToAction(nameof(Index));
        }

        // Урок 10
        // GET: Index/Remove/Id
        public IActionResult Remove(int id)
        {
            // Получаем массив id корзины
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("sShoppingCart");

            // Проверяем, есть ли элементы в полученном массиве
            if (listShoppingCart.Count > 0)
            {
                // Проверяем, содержится ли в полученном массиве переданный в метод id
                if (listShoppingCart.Contains(id))
                    listShoppingCart.Remove(id);
            }

            // Обновляем данные в сессии
            HttpContext.Session.Set("sShoppingCart", listShoppingCart);

            // Устанавливаем сообщение о успешном удалении
            TempData["SM"] = "Product removed from your cart";

            // Переадресовываем пользователя на страницу Index
            return RedirectToAction(nameof(Index));
        }
    }
}