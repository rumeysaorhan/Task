using DocumentFormat.OpenXml.Wordprocessing;
using MepasTask.App.Interfaces;
using MepasTask.Models;
using MepasTask.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace MepasTask.UI.Controllers
{
    public class ProductsController : Controller
    {
        //private readonly IUnitOfWork unitOfWork;

        public ProductsController()//IUnitOfWork unitOfWork)
        {
            //this.unitOfWork = unitOfWork;
        }

        //Tüm ürünleri listeleme sayfası
        public async Task<IActionResult> Index()
        {
            try
            {
              
                var list = API<ProductsModel>.SelectAsyncToList("Products", "ListProduct", null).Result.ToList();

                if (list != null)
                {
                   
                    return View(list);
                }
                else
                {
                    ViewBag.Error = "Dosyalar listelenirken bir hata oluştu.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Bir hata oluştu: {ex.Message}";
                return View();
            }
        }

        //Kategoriye göre listeleme sayfası
        public async Task<IActionResult> Categories(int kategoriid)
        {
            try
            {
                var qs = new Dictionary<string, string>()
                {
                    ["kategoriid"] = kategoriid.ToString()
                };
                var list = API<ProductsModel>.SelectAsyncToList("Products", "ListProductCategories", qs).Result.ToList();

                if (list != null)
                {

                    return View(list);
                }
                else
                {
                    ViewBag.Error = "Dosyalar listelenirken bir hata oluştu.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Bir hata oluştu: {ex.Message}";
                return View();
            }
        }
       
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {

            if (id == 0)
            {
                return View(new ProductsModel());
            }
            else
            {
                var qs = new Dictionary<string, string>()
                {
                    ["id"] = id.ToString()
                };
                var model = API<ProductsModel>.SelectAsyncFirstOrDefault("Products", "GetByID", qs).Result;
                if (model == null)
                {
                    return NotFound();
                }
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(ProductsModel model)
        {
            model.added_user_id = 1;
            model.updated_user_id = 1;
            model.created_date = DateTime.Today;
            model.updated_date = DateTime.Today;
            bool suc = API<ProductsModel>.SaveAsync("Products", "AddOrEdit", model).Result;
            if (suc)
            {
                return RedirectToAction("Index");
            }
            else return Json(new { success = 0, message = "İşlem sırasında bir hata oluştu!" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                bool suc = await API<ProductsModel>.DeleteAsync("Products", "Delete", id);
                if (suc)
                {
                    return RedirectToAction("Index");

                }
                else
                {
                    return Json(new { success = 0, message = "İşlem sırasında bir hata oluştu!" });

                }
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, message = "İşlem sırasında bir hata oluştu!" });
            }
        }





    }
}
