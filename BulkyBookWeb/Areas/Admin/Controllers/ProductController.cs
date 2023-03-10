using BulkyBookWeb.Data;
using BulkyBook.Models1;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess1.Repository.IRepository;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //GET
        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverTypeList = _unitOfWork.CoverType.GetAll();
            return View(objCoverTypeList);
        }


        //GET edit
        public IActionResult Upsert(int? id)
		{

            Product product = new();
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(
            u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            IEnumerable<SelectListItem> CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
            u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            if (id == null || id == 0)
			{
                //esempio di uso di ViewBag
                ViewBag.CategoryList = CategoryList;
                //esempio di uso di ViewData
                ViewData["CoverTypeList"] = CoverTypeList
                return View(product);
            }
            return View(product);

        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
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
            var categoryFromDbFirst = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
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
            var categoryFromDbFirst = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

            if (categoryFromDbFirst == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(categoryFromDbFirst);
            _unitOfWork.Save();
            TempData["success"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}