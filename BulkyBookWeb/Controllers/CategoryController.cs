using BulkyBookWeb.Data;
using BulkyBook.Models1;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess1.Repository.IRepository;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _db;
        public CategoryController(ICategoryRepository db)
        {
            _db = db;
        }
        //private readonly ApplicationDbContext _db;

        //public CategoryController(ApplicationDbContext db)
        //{
        //    _db = db;
        //}
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.GetAll();
            return View(objCategoryList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST          //aggiunge i dati nel database
        [HttpPost]
        [ValidateAntiForgeryToken]  //evita che l'utente lo crei dall'url
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError(nameof(obj.Name), $"The name of property {nameof(obj.DisplayOrder)} cannot exactly match the name of property { nameof(obj.Name)}");
            }
            if (ModelState.IsValid) //validazione
            {
                _db.Add(obj);
                _db.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);

        }

		//GET edit
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
            //var categoryFromDb = _db.Categories.Find(id);
            //var categoryFromDbFirst = _db.GetFirstOrDefault(u => u.Id == id);

            var categoryFromDbFirst = _db.GetFirstOrDefault(u => u.Id == id);

            if (categoryFromDbFirst == null)
            {
                return NotFound();
            }
            return View(categoryFromDbFirst);

        }

        //POST
        [HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Category obj)
		{
			if (obj.Name == obj.DisplayOrder.ToString())
			{
				ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
			}
			if (ModelState.IsValid)
			{
                _db.Update(obj);
                _db.Save();

                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
			}
			return View(obj);
		}

        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //var categoryFromDb = _db.Categories.Find(id);
            var categoryFromDbFirst = _db.GetFirstOrDefault(u => u.Id == id);
            if (categoryFromDbFirst == null)
            {
                return NotFound();
            }
            return View(categoryFromDbFirst);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id, [Bind("Id")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            //var obj = _db.Categories.Find(id);
            var categoryFromDbFirst = _db.GetFirstOrDefault(u => u.Id == id);
            if (categoryFromDbFirst == null)
            {
                return NotFound();
            }
            //_db.Categories.Remove(categoryFromDbFirst);
            //if (obj == null)
            //{
            //    return NotFound();
            //}
            //_db.Categories.Remove(obj);
            //_db.SaveChanges();
            _db.Remove(categoryFromDbFirst);
            _db.Save();
            TempData["success"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }

    }

}
