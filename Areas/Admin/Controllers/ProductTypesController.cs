using System.Linq;
using System.Threading.Tasks;
using CoreStoreMVC.Data;
using CoreStoreMVC.Models;
using CoreStoreMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreStoreMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area(nameof(Admin))]
    public class ProductTypesController : Controller
    {
        // Создаём переменную только для чтения, чтобы использовать базу данных
        private readonly ApplicationDbContext _db;
        // Внедряем зависимость базы данных через конструктор класса
        public ProductTypesController(ApplicationDbContext db)
        {
            // Предаём все данные в приватную переменную
            _db = db;
        }
        // GET метод представления Index
        public IActionResult Index()
        {
            // Задача - получить и вернуть в представление все типы продуктов в листе

            // Решение задачи
            return View(_db.ProductsTypes.ToList());
        }

        // GET метод создания типов продуктов
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST метод создания типов продуктов
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductsType productsType)
        {
            // 1. Проверяем модель на валидность
            if (ModelState.IsValid)
            {
                // 1.1 Если модель валидная, то добавляем значение модели в сущности Entity Framework
                _db.Add(productsType);

                // 1.2 Сохраняем все изменения в базе данных асинхронно
                await _db.SaveChangesAsync();

                // 1.4 Добавляем сообщение о успешном добавлении типа в TempData
                TempData["SM"] = $"Product type: {productsType.Name} added successful!";

                // 1.3 Переадресовываем на страницу вывода всех типов продуктов
                return RedirectToAction(nameof(Index));
            }

            // 2. Если модель не валидная, возвращаем текущее представление с моделью для исправления ошибок
            return View(productsType);
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var productType = await _db.ProductsTypes.FindAsync(id);
            if (productType == null)
                return NotFound();

            return View(productType);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductsType productsType)
        {
            if (id != productsType.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(productsType);

            _db.Update(productsType);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Product type {productsType.Name} editing successful.";

            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var productType = await _db.ProductsTypes.FindAsync(id);
            if (productType == null)
                return NotFound();

            return View(productType);
        }

      
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var productType = await _db.ProductsTypes.FindAsync(id);

            if (productType == null)
                return NotFound();

            return View(productType);
        }
       
        // Указываем официальное имя метода через аннотацию
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Получаем тип продукта из базы и записываем её в переменную
            var productType = await _db.ProductsTypes.FindAsync(id);

            // Проверяем, найден ли тип продукта или получен NULL
            if (productType != null)
            {
                // Удаляем через сущности Entity
                _db.ProductsTypes.Remove(productType);

                // Сохраняем изменения
                await _db.SaveChangesAsync();

                // Добавляем сообщение о успешном удалении типа продукта
                TempData["SM"] = $"Product type {productType.Name} deleting successful.";
            }
            else
                TempData["SM"] = $"Product type {productType.Name} deleting FAILED.";
            
            // Переадресовываем на страницу Index
            return RedirectToAction(nameof(Index));
        }
    }
}
