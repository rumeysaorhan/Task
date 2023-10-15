using Microsoft.AspNetCore.Mvc;
using MepasTask.Models;
using MepasTask.API.Helpers;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using MepasTask.UI.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using MepasTask.App.Interfaces;
using MepasTask.Infrastructure.Repository;
using MepasTask.Infrastructure;
using NPOI.SS.Formula.Functions;
using DocumentFormat.OpenXml.EMMA;

namespace MepasTask.API.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        public readonly IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        // private readonly IUnitOfWork unitOfWork;

        public ProductsController(IWebHostEnvironment _environment, IConfiguration _configuration)//, IUnitOfWork unitOfWork)
        {
            Environment = _environment;
            Configuration = _configuration;
            // this.unitOfWork = unitOfWork;
        }

        //Tüm ürünleri listeleme sayfası
        [HttpGet("ListProduct")]
        public async Task<IActionResult> ListProduct()
        {
            try
            {
                string webRootPath = Environment.WebRootPath;
                string filePath = Path.Combine(webRootPath, "UploadedFolders", "veritabani.xlsx");
                //var model = await unitOfWork.Products.GetAllAsync(filePath);

                List<ProductsModel> productList = ReadProductsFromExcel(filePath);
                return Ok(productList);
            }
            catch (Exception ex)
            {
                return BadRequest("Hata: Ekleme işlemi başarısız oldu.");
            }
        }

        //Kategoriye göre listeleme sayfası
        [HttpGet("ListProductCategories")]
        public async Task<IActionResult> ListProductCategories(int kategoriid)
        {
            try
            {
                string webRootPath = Environment.WebRootPath;
                string filePath = Path.Combine(webRootPath, "UploadedFolders", "veritabani.xlsx");
                List<ProductsModel> productList = ReadProductsFromExcelCategories(filePath, kategoriid);
                return Ok(productList);
            }
            catch (Exception ex)
            {
                return BadRequest("Hata: Ekleme işlemi başarısız oldu.");
            }
        }

        //id ye göre product çekme
        [HttpGet("GetByID")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                string webRootPath = Environment.WebRootPath;
                string filePath = Path.Combine(webRootPath, "UploadedFolders", "veritabani.xlsx");
                ProductsModel product = ReadProductsFromExcelById(filePath, id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest("Hata: Ekleme işlemi başarısız oldu.");
            }
        }

        //düzenlenen ya da güncellenen modelin posttu
        [HttpPost("AddOrEdit")]
        public async Task<IActionResult> AddOrEdit(ProductsModel model)
        {
            string webRootPath = Environment.WebRootPath;
            string filePath = Path.Combine(webRootPath, "UploadedFolders", "veritabani.xlsx");

            try
            {
                bool suc = InsertOrUpdateProductInExcel(model, filePath);
                if (suc)
                {
                    return Ok("Başarı ile kaydedildi.");

                }
                else return BadRequest("İşlem sırasında bir hata meydana geldi.");

            }
            catch (Exception ex)
            {
                return BadRequest("İşlem sırasında bir hata meydana geldi.");
            }

        }
        [HttpGet("InsertOrUpdateProductInExcel")]
        public bool InsertOrUpdateProductInExcel(ProductsModel product, string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    IWorkbook workbook = new XSSFWorkbook(fs);
                    ISheet sheet = workbook.GetSheetAt(0); // İlk çalışma sayfasını al

                    bool productUpdated = false;

                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        IRow excelRow = sheet.GetRow(row);

                        int Id = (int)excelRow.GetCell(0).NumericCellValue; // id'yi alır

                        // Eğer id= id ise, ürünü güncelle
                        if (Id == product.id)
                        { 
                           
                            excelRow.GetCell(1).SetCellValue(product.name);
                            excelRow.GetCell(2).SetCellValue(product.category_id);
                            excelRow.GetCell(3).SetCellValue((double)product.price);
                            excelRow.GetCell(4).SetCellValue(product.unit);
                            excelRow.GetCell(5).SetCellValue(product.stock);
                            excelRow.GetCell(6).SetCellValue(product.color);
                            excelRow.GetCell(7).SetCellValue((int)product.weight);
                            excelRow.GetCell(8).SetCellValue((int)product.width);
                            excelRow.GetCell(9).SetCellValue((int)product.heigth);
                            excelRow.GetCell(10).SetCellValue(product.added_user_id);
                            excelRow.GetCell(11).SetCellValue((int)product.updated_user_id);
                            excelRow.GetCell(12).SetCellValue((DateTime)product.created_date);
                            excelRow.GetCell(13).SetCellValue((DateTime)product.updated_date);

                            productUpdated = true;
                            break;
                        }
                    }

                    // Eğer ürün güncellenmezse, yeni bir satır ekleyin
                    if (!productUpdated)
                    {
                        int maxId = 0;

                        for (int row = 1; row <= sheet.LastRowNum; row++)
                        {
                            IRow excelRow = sheet.GetRow(row);
                            int currentId = (int)excelRow.GetCell(0).NumericCellValue;
                            if (currentId > maxId)
                            {
                                maxId = currentId;
                            }
                        }
                        int newId = maxId + 1;

                        IRow newRow = sheet.CreateRow(sheet.LastRowNum + 1);

                        newRow.CreateCell(0).SetCellValue(newId); // Yeni Id
                        newRow.CreateCell(1).SetCellValue(product.name);
                        newRow.CreateCell(2).SetCellValue(product.category_id);
                        newRow.CreateCell(3).SetCellValue((double)product.price);
                        newRow.CreateCell(4).SetCellValue(product.unit);
                        newRow.CreateCell(5).SetCellValue(product.stock);
                        newRow.CreateCell(6).SetCellValue(product.color);
                        newRow.CreateCell(7).SetCellValue((int)product.weight);
                        newRow.CreateCell(8).SetCellValue((int)product.width);
                        newRow.CreateCell(9).SetCellValue((int)product.heigth);
                        newRow.CreateCell(10).SetCellValue(product.added_user_id);
                        newRow.CreateCell(11).SetCellValue((int)product.updated_user_id);
                        newRow.CreateCell(12).SetCellValue((DateTime)product.created_date);
                        newRow.CreateCell(13).SetCellValue((DateTime)product.updated_date);
                    }

                    // Dosyayı kaydet
                    using (FileStream fsWrite = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        workbook.Write(fsWrite);
                    }

                    return true; // Başarı durumu
                }
            }
            catch (Exception ex)
            {
                return false; // Başarısızlık durumu
            }
        }

        //Kayıt silmek için
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                if (id == 0)
                {
                    return NotFound();
                }
                string webRootPath = Environment.WebRootPath;
                string filePath = Path.Combine(webRootPath, "UploadedFolders", "veritabani.xlsx");

                bool suc = DeleteProductInExcel(id, filePath);
                if (suc)
                {
                    return Ok("Başarı ile kaydedildi.");

                }
                else return BadRequest("İşlem sırasında bir hata meydana geldi.");
            }

            catch (Exception ex)
            {
                return BadRequest("İşlem sırasında bir hata meydana geldi.");
            }
        }
        [HttpGet("DeleteProductInExcel")]
        public bool DeleteProductInExcel(int id, string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    IWorkbook workbook = new XSSFWorkbook(fs);
                    ISheet sheet = workbook.GetSheetAt(0); // İlk çalışma sayfasını al
                    
                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        IRow excelRow = sheet.GetRow(row);

                        int Id = (int)excelRow.GetCell(0).NumericCellValue; // id'yi alır

                        // Eğer id= id ise, satırı sil
                        if (Id == id)
                        {
                            sheet.RemoveRow(excelRow);
                            break;
                        }
                    }

                    // Dosyayı kaydet
                    using (FileStream fsWrite = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        workbook.Write(fsWrite);
                    }

                    return true; // Başarı durumu
                }
            }
            catch (Exception ex)
            {
                return false; // Başarısızlık durumu
            }
        }

        //Exceldeki tüm verileri çekmek ve modele yazdırmak için
        [HttpGet("ReadProductsFromExcel")]
        public List<ProductsModel> ReadProductsFromExcel(string filePath)
        {
            List<ProductsModel> productList = new List<ProductsModel>();

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                ISheet sheet = workbook.GetSheetAt(0); // İlk çalışma sayfasını al

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    IRow excelRow = sheet.GetRow(row);

                    productList.Add(new ProductsModel
                    {
                        id = (int)excelRow.GetCell(0).NumericCellValue,
                        name = excelRow.GetCell(1).StringCellValue,
                        category_id = (int)excelRow.GetCell(2).NumericCellValue,
                        price = (decimal)excelRow.GetCell(3).NumericCellValue,
                        unit = excelRow.GetCell(4).StringCellValue,
                        stock = (int)excelRow.GetCell(5).NumericCellValue,
                        color = excelRow.GetCell(6).StringCellValue,
                        weight = (int)excelRow.GetCell(7).NumericCellValue,
                        width = (int)excelRow.GetCell(8).NumericCellValue,
                        heigth = (int)excelRow.GetCell(9).NumericCellValue,
                        added_user_id = (int)excelRow.GetCell(10).NumericCellValue,
                        updated_user_id = (int)excelRow.GetCell(11).NumericCellValue,
                        created_date = excelRow.GetCell(12).DateCellValue,
                        updated_date = excelRow.GetCell(13).DateCellValue
                    });
                }
            }

            return productList;
        }
        //Exceldeki kategori id ye göre olan verileri çekmek ve modele yazdırmak için

        [HttpGet("ReadProductsFromExcelCategories")]
        public List<ProductsModel> ReadProductsFromExcelCategories(string filePath, int kategoriid)
        {
            List<ProductsModel> productList = new List<ProductsModel>();

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                ISheet sheet = workbook.GetSheetAt(0); // İlk çalışma sayfasını alır

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    IRow excelRow = sheet.GetRow(row);

                    int categoryId = (int)excelRow.GetCell(2).NumericCellValue; // category_id'yi alır

                    // Eğer category_id =kategoriid ise, ürünü ekle
                    if (categoryId == kategoriid)
                    {
                        productList.Add(new ProductsModel
                        {
                            id = (int)excelRow.GetCell(0).NumericCellValue,
                            name = excelRow.GetCell(1).StringCellValue,
                            category_id = categoryId,
                            price = (decimal)excelRow.GetCell(3).NumericCellValue,
                            unit = excelRow.GetCell(4).StringCellValue,
                            stock = (int)excelRow.GetCell(5).NumericCellValue,
                            color = excelRow.GetCell(6).StringCellValue,
                            weight = (int)excelRow.GetCell(7).NumericCellValue,
                            width = (int)excelRow.GetCell(8).NumericCellValue,
                            heigth = (int)excelRow.GetCell(9).NumericCellValue,
                            added_user_id = (int)excelRow.GetCell(10).NumericCellValue,
                            updated_user_id = (int)excelRow.GetCell(11).NumericCellValue,
                            created_date = excelRow.GetCell(12).DateCellValue,
                            updated_date = excelRow.GetCell(13).DateCellValue
                        });
                    }
                }
            }

            return productList;
        }
        //Exceldeki  id ye göre olan verileri çekmek ve modele yazdırmak için

        [HttpGet("ReadProductsFromExcelById")]
        public ProductsModel ReadProductsFromExcelById(string filePath, int id)
        {
            ProductsModel product = new ProductsModel();

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                ISheet sheet = workbook.GetSheetAt(0); // İlk çalışma sayfasını alır

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    IRow excelRow = sheet.GetRow(row);

                    int Id = (int)excelRow.GetCell(0).NumericCellValue; // id'yi alır

                    // Eğer id= id ise, ürünü ekle
                    if (Id == id)
                    {
                        product = new ProductsModel
                        {
                            id = Id,
                            name = excelRow.GetCell(1).StringCellValue,
                            category_id = (int)excelRow.GetCell(2).NumericCellValue,
                            price = (decimal)excelRow.GetCell(3).NumericCellValue,
                            unit = excelRow.GetCell(4).StringCellValue,
                            stock = (int)excelRow.GetCell(5).NumericCellValue,
                            color = excelRow.GetCell(6).StringCellValue,
                            weight = (int)excelRow.GetCell(7).NumericCellValue,
                            width = (int)excelRow.GetCell(8).NumericCellValue,
                            heigth = (int)excelRow.GetCell(9).NumericCellValue,
                            added_user_id = (int)excelRow.GetCell(10).NumericCellValue,
                            updated_user_id = (int)excelRow.GetCell(11).NumericCellValue,
                            created_date = excelRow.GetCell(12).DateCellValue,
                            updated_date = excelRow.GetCell(13).DateCellValue
                        };
                        break; // Ürünü bulduktan sonra döngüyü kırmak için
                    }
                }
            }

            return product;
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> FileUpload(IFormFile uFile)
        {
            DataTable dtResult = new DataTable();
            ReadWriteData rd = new ReadWriteData(Environment, Configuration);

            string filename = "veritabani";

            string savedFileName = await rd.SaveFileToFolder(uFile, "UploadedFolders", filename);
            if (savedFileName != "error")
            {
                try
                {
                    dtResult = rd.ReadExcelToDataTable("UploadedFolders", savedFileName);

                    if (dtResult != null)
                    {

                        List<tmpexcel> list = Helpers.ModelHelper.DataTableToList<tmpexcel>(dtResult);

                        return Ok(list);


                    }
                    else
                    {
                        return BadRequest("Hata: Ekleme işlemi başarısız oldu.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Hata: Ekleme işlemi başarısız oldu.");
                }

            }
            else
            {
                return BadRequest("Hata: Ekleme işlemi başarısız oldu.");
            }

        }

    }
}
