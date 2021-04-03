using System;
using System.Collections.Generic;
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
    public class SpecialTagsController : Controller
    {
        // Создаём переменную только для чтения, чтобы использовать базу данных
        private readonly ApplicationDbContext _db;
        // Внедряем зависимость базы данных через конструктор класса
        public SpecialTagsController(ApplicationDbContext db)
        {
            // Предаём все данные в приватную переменную
            _db = db;
        }
        // GET метод представления Index
        public IActionResult Index()
        {
            
            return View(_db.SpecialTags.ToList());
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
        public async Task<IActionResult> Create(SpecialTag specialTag)
        {
            // 1. Проверяем модель на валидность
            if (ModelState.IsValid)
            {
                // 1.1 Если модель валидная, то добавляем значение модели в сущности Entity Framework
                _db.Add(specialTag);

                // 1.2 Сохраняем все изменения в базе данных асинхронно
                await _db.SaveChangesAsync();

                // 1.4 Добавляем сообщение о успешном добавлении типа в TempData
                TempData["SM"] = $"Special tag: {specialTag.Name} added successful!";

                // 1.3 Переадресовываем на страницу вывода всех типов продуктов
                return RedirectToAction(nameof(Index));
            }

            // 2. Если модель не валидная, возвращаем текущее представление с моделью для исправления ошибок
            return View(specialTag);
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTags.FindAsync(id);
            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTag specialTag)
        {
            if (id != specialTag.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(specialTag);

            _db.Update(specialTag);
            await _db.SaveChangesAsync();

            TempData["SM"] = $"Special tag {specialTag.Name} editing successful.";

            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTags.FindAsync(id);
            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }

        
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var specialTag = await _db.SpecialTags.FindAsync(id);

            if (specialTag == null)
                return NotFound();

            return View(specialTag);
        }
        
        // Указываем официальное имя метода через аннотацию
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Получаем тип продукта из базы и записываем её в переменную
            var specialTag = await _db.SpecialTags.FindAsync(id);

            // Проверяем, найден ли тип продукта или получен NULL
            if (specialTag != null)
            {
                // Удаляем через сущности Entity
                _db.SpecialTags.Remove(specialTag);

                // Сохраняем изменения
                await _db.SaveChangesAsync();

                // Добавляем сообщение о успешном удалении типа продукта
                TempData["SM"] = $"Special tag {specialTag.Name} deleting successful.";
            }
            else
                TempData["SM"] = $"Special tag {specialTag.Name} deleting FAILED.";
            
            // Переадресовываем на страницу Index
            return RedirectToAction(nameof(Index));
        }
    }
}
