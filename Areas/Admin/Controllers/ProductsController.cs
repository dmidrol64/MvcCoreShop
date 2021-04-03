using CoreStoreMVC.Data;
using CoreStoreMVC.Models;
using CoreStoreMVC.Models.ViewModel;
using CoreStoreMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// Урок 4
namespace CoreStoreMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area(nameof(Admin))]
    public class ProductsController : Controller
    {
        
        // 1. Внедряем зависимость базы данных
        private readonly ApplicationDbContext _db;

        
        // V. Добавляем переменную для получения пути к корню сайта
        private readonly IWebHostEnvironment _hostingEnvironment;

        
        // 2. Добавляем свойство с атрибутом для всего контроллера
        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }

        public ProductsController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;

            
            // 3. Инициализируем в конструкторе свойство значениями
            ProductsVM = new ProductsViewModel()
            {
                Products = new Product(),
                ProductTypes = _db.ProductsTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList()
            };
        }

       
        // 4. Реализовать метод
        public async Task<IActionResult> Index()
        {
            // Заполняем модель данными, в том числе и из связанных таблиц
            var products = _db.Products.Include(x => x.ProductTypes).Include(x => x.SpecialTags);
            return View(await products.ToListAsync());
        }

        
        // 1. Добавляем метод Create
        // GET: Products/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(ProductsVM);
        }

       
        // 2. Добавляем POST метод Create
        // POST: Products/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            // I. Проверить модель на валидность
            if (!ModelState.IsValid)
                return View(ProductsVM);

            // II. Если модель валидна, добавляем информацию в сущности
            await _db.Products.AddAsync(ProductsVM.Products);
            // III. Сохраняем изменения в базе данных
            await _db.SaveChangesAsync();

            // IV. Логика сохранения картинок
            // Image begin saved
            string webRootPath = _hostingEnvironment.WebRootPath;

            // На этом месте СОЗДАТЬ папку в главном проекте Utility

            // Получаем файлы из формы
            var files = HttpContext.Request.Form.Files;

            // Получаем товары из базы данных
            var productsFromDb = await _db.Products.FindAsync(ProductsVM.Products.Id);

            // Проверяем, получены ли какие-то файлы
            if (files.Count != 0)
            {
                // Image has been uploaded
                // I. Комбинируем путь к папке сохранения
                var uploadPath = Path.Combine(webRootPath, SD.ImageFolder);

                // II. Получаем расширение полученного файла
                var extension = Path.GetExtension(files[0].FileName);

                // III. Сохраняем картинки
                using (var fileStream = new FileStream(Path.Combine(uploadPath, ProductsVM.Products.Id + extension), FileMode.Create))
                {
                    await files[0].CopyToAsync(fileStream);
                }

                // IV. Записываем в базу путь к сохранённой картинке
                productsFromDb.Image = $"\\{SD.ImageFolder}\\{ProductsVM.Products.Id}{extension}";
            }
            else
            {
                // V. На этом этапе добавляем в папку ProductImage default_product.png
                // Формируем путь к изображению по умолчанию
                var uploadPath = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);

                // Копируем картинку в папку изображения продукта
                System.IO.File.Copy(uploadPath, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png");
                // Сохраняем путь к картинке в модель
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png";
            }

            // VI. Сохраняем все изменения в базе
            await _db.SaveChangesAsync();

            // Добавляем сообщение о удачном добавлении
            TempData["SM"] = $"Product {ProductsVM.Products.Name} adding successful!";

            // Переадресовываем пользователя на страницу Index
            return RedirectToAction(nameof(Index));
        }

        
        // GET: Products/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            // 1. Проверяем полученный ID на null
            if (id == null)
                return NotFound();

            // 2. Заполняем модель представления данными
            // У нас есть 3 варианта, как завершить запрос: FirstOrDefault() , SingleOrDefault() , FindAsync()
            // В большинстве скаффолдингового кода FindAsync может использоваться вместо FirstOrDefaultAsync.
            // SingleOrDefaultAsync извлекает больше данных и выполняет ненужную работу. выдает исключение, если существует более одного объекта, который соответствует части фильтра.
            // FirstOrDefaultAsync не выбрасывает, если существует более одного объекта, который соответствует части фильтра.
            ProductsVM.Products = await _db.Products
                                           .Include(x => x.ProductTypes)
                                           .Include(x => x.SpecialTags)
                                           .FirstOrDefaultAsync(x => x.Id == id);

            // 3. Проверяем, найдены ли данные
            if (ProductsVM.Products is null)
                return NotFound();

            // 4. Если модель найдена, возвращаем представление с моделью
            return View(ProductsVM);
        }

        
        // POST: Products/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            // 1. Проверить модель на валидность
            if (!ModelState.IsValid)
                return View(ProductsVM);

            // 2. Создаём необходимые переменные для путей картинки (webRootPath и file)
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            // 3. Получить и сохранить в отдельную переменную редактируемый товар из базы данных (это надо для обработки изображения)
            var productFromDb = _db.Products.FirstOrDefault(x => x.Id == ProductsVM.Products.Id);

            // 4. Проверяем, получены ли файлы из формы и не null ли переменная files
            if (files.Count > 0 && files[0] != null)
            {
                // 4.1 Если проверка успешно пройдена, то создаём путь к папке с изображениями
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);

                // 4.2 Получаем новое расширение файла
                var extension_new = Path.GetExtension(files[0].FileName);

                // 4.3 Получаем старое расширение файла (из переменной productFromDb)
                var extension_old = Path.GetExtension(productFromDb.Image);

                // 4.4 Проверяем, имеется ли уже в наличии файл (метод System.IO.File.Exists(Path.Combine(путь_загрузки, ID_продукта + старое_расширение_файла)))
                if (System.IO.File.Exists(Path.Combine(uploads, ProductsVM.Products.Id + extension_old)))
                    // 4.4.1 Если файл имеется, то удалить его
                    System.IO.File.Delete(Path.Combine(uploads, ProductsVM.Products.Id + extension_old));

                // 4.5 После чего - копируем новый
                using (var fileStream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension_new), FileMode.Create))
                {
                    await files[0].CopyToAsync(fileStream);
                }

                // 4.6 Сохраняем путь в модель представления
                ProductsVM.Products.Image = $"\\{SD.ImageFolder}\\{ProductsVM.Products.Id}{extension_new}";
            }

            // 5. Проверяем, если путь к картинке в модели не равен NULL
            if (ProductsVM.Products.Image != null)
                // 5.1 Сохраняем путь картинки в модели, полученной из базы данных (productFromDb)
                productFromDb.Image = ProductsVM.Products.Image;

            // 6. Обновляем все свойства модели productFromDb на полученные из ProductsVM
            productFromDb.Name = ProductsVM.Products.Name;
            productFromDb.Price = ProductsVM.Products.Price;
            productFromDb.Available = ProductsVM.Products.Available;
            productFromDb.ProductTypeId = ProductsVM.Products.ProductTypeId;
            productFromDb.SpecialTagId = ProductsVM.Products.SpecialTagId;
            productFromDb.ShadeColor = ProductsVM.Products.ShadeColor;

            // 7. Сохранить все изменения в базе данных
            await _db.SaveChangesAsync();

            // 8. Добавить сообщение об успешном обновлении
            TempData["SM"] = $"Product {ProductsVM.Products.Name} update successful!";

            // 9. Переадресовать пользователя на страницу Index
            return RedirectToAction(nameof(Index));
        }

        
        // GET: Products/Details
        public async Task<IActionResult> Details(int? id)
        {
            // 1. Проверяем полученный ID на null
            if (id == null)
                return NotFound();

            // 2. Заполняем модель представления данными
            ProductsVM.Products = await _db.Products
                                           .Include(x => x.ProductTypes)
                                           .Include(x => x.SpecialTags)
                                           .FirstOrDefaultAsync(x => x.Id == id);

            // 3. Проверяем, найдены ли данные
            if (ProductsVM.Products is null)
                return NotFound();

            // 4. Если модель найдена, возвращаем представление с моделью
            return View(ProductsVM);
        }

       
        // GET: Products/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            // 1. Проверяем полученный ID на null
            if (id == null)
                return NotFound();

            // 2. Заполняем модель представления данными
            ProductsVM.Products = await _db.Products
                                           .Include(x => x.ProductTypes)
                                           .Include(x => x.SpecialTags)
                                           .FirstOrDefaultAsync(x => x.Id == id);

            // 3. Проверяем, найдены ли данные
            if (ProductsVM.Products is null)
                return NotFound();

            // 4. Если модель найдена, возвращаем представление с моделью
            return View(ProductsVM);
        }

        
        // POST: Products/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 1. Получить и сохранить в переменную путь к корневой директории (wwwroot)
            string webRootPath = _hostingEnvironment.WebRootPath;

            // 2. Получить данные и сохранить в переменную (класс Product) единичную модель из базы данных по полученному, через параметр, id (асинхронно или синхронно?)
            Product product = await _db.Products.FindAsync(id);

            // 3. Проверить, найден ли товар или вернулся null
            if (product == null)
                // 3.1 Если вернулся null, то переадресовать на страницу NotFound 
                return NotFound();
            // 4. В противном случаи (else)
            else
            {
                // 4.1 Комбинируем и сохраняем в отдельную переменную путь до каталога загрузки картинок (webRootPath, SD.ImageFolder)
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);

                // 4.2 Получить и сохранить в переменную расширение изображения текущего продукта (метод Path.GetExtension())
                var extension = Path.GetExtension(product.Image);

                // 4.3 Проверить, существует ли картинка текущего продукта (Метод: System.IO.Exsists(Path.Combine(путь_к_каталогу_картинок, ид_продукта + расширение))))
                if (System.IO.File.Exists(Path.Combine(uploads, product.Id + extension)))
                    // 4.3.1 Если существует, то удалить (Тот же метод, что и при проверке, но Exists заменить на Delete)
                    System.IO.File.Delete(Path.Combine(uploads, product.Id + extension));

                // 4.4 Удаляем сам продукт из сущностей Entity
                _db.Products.Remove(product);

                // 4.5 Сохраняем изменения в базе данных (синхронно или асинхронно?)
                await _db.SaveChangesAsync();

                // 4.6 Добавляем сообщение о успешном удалении
                TempData["SM"] = $"Product: {product.Name} deleting successful!";

                // 4.7 Переадресовываем пользователя на страницу Index
                return RedirectToAction(nameof(Index));
            }
            
        }
    }
}
