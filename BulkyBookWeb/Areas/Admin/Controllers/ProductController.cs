using BulkyBook.DataAccess1.Repository.IRepository;
using BulkyBook.Models1.ViewModels;
using BulkyBook.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;

        //CreatedResult post 
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        #region UPSERT ACTION

        //GET UPSERT 
        public IActionResult Upsert(int? id)
        {

            //popolamento di elementi html di tipo select ner form per la creazione di un nuovo prodotto 

            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            #region Upsert vecchio
            //if (id == null || id == 0)
            //{
            //    //create product
            //    return View(productVM);
            //}
            //else
            //{
            //    //update product
            //}
            //return View(productVM); 
            #endregion

            if (id == null || id == 0)
            {
                //restituisce una view per la creazione di un nuovo prodotto
                return View(productVM);
            }
            else
            {
                var productInDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
                if (productInDb != null)
                {
                    productVM.Product = productInDb;
                    //restituisce una view per l'aggiornamento del prodotto
                    //questa view riceve un productVM con tutti i campi di Product
                    return View(productVM);
                }
                //il prodotto con l'id inviato non è stato trovato nel database.
                //restituisce una view per creare un nuovo prodotto
                return View(productVM);



            }
        }



        //POST UPSERT 
        //uso dell'interfaccia virtuale 
        [HttpPost]
        [ValidateAntiForgeryToken] //viene mandato un token e viene paragonato a quello del controller 
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {

            if (ModelState.IsValid) //controlla i dati che metti sono congruenti con quelli che metto nel modello Model
            {
                string wwwRootPath = _hostEnvironment.WebRootPath; //prende il percroso del file
                if (file != null)
                {
                    //creiamo un nuovo nome per il file che l'utente ha caricato
                    //facciamo in modo che non possano esistere due file con lo stesso nome
                    string fileName = Guid.NewGuid().ToString();//genera stringa alfanumerica
                    var uploadDir = Path.Combine(wwwRootPath, "images", "products"); //combina il wwwroot con image e products
                    var fileExtension = Path.GetExtension(file.FileName); //estesione del file 
                    //nel caso di upload dell'immagine del prodotto, il precedente file, se esiste, deve essere rimosso
                    if (obj.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart(Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var filePath = Path.Combine(uploadDir, fileName + fileExtension);//prende la carrtella upoad e gli aggancia l'estensione
                    var fileUrlString = filePath[wwwRootPath.Length..].Replace(@"\\", @"\"); // fa replace e la restituzione di caratteri

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }


                    obj.Product.ImageUrl = fileUrlString;

                }

                if (obj.Product.Id == 0)//new Product
                {
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["success"] = "Product created successfully";
                }
                else //update exsisting Product
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product updated successfully";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(obj);

        }


        #endregion





        //GET EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var productFromDbFirst = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (productFromDbFirst == null)
            {
                return NotFound();
            }
            return View(productFromDbFirst);

        }

        //POST EDIT 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj); //aggiornare 
                _unitOfWork.Save();
                TempData["Success"] = "Product EDIT success";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }


        #region DELETE ACTION
        // DELETE GET 
        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var objFromDbFirst = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (objFromDbFirst == null)//l'oggetto con l'id specificato non è stato trovato
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else //l'oggetto con l'id specificato è stato trovato
            {
                if (objFromDbFirst.ImageUrl != null) //l'oggetto ha un ImageUrl!=null
                {
                    var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, objFromDbFirst.ImageUrl.TrimStart(Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldImagePath))//se il file corrispondente all'ImageUrl esiste va eliminato
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.Product.Remove(objFromDbFirst);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Delete Successful" });
            }



        }

        #endregion


        public IActionResult Index()
        {
            return View();
        }


        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productList });

        }
        #endregion



    }
}


