using MepasTask.App.Interfaces;
using MepasTask.Models;
using Microsoft.Extensions.Configuration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;



namespace MepasTask.Infrastructure.Repository
{
    public class ProductsRepository :  IProductsRepository
    {
        private readonly IConfiguration configuration;
         
        public ProductsRepository(IConfiguration configuration)// : base(configuration, typeof(ProductsModel).Name)
        {
            this.configuration = configuration;
        }
        //Exceldeki tüm verileri çekmek ve modele yazdırmak için
        public async Task<IReadOnlyList<ProductsModel>> GetAllAsync(string filePath)
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

        }
}