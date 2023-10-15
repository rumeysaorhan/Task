using MepasTask.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using MepasTask.UI.Models;
using MepasTask.Models;
using MepasTask.API.Helpers;

namespace MepasTask.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class LoginController : ControllerBase
    {
        //private readonly IUnitOfWork unitOfWork;
        public readonly IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        public LoginController(IWebHostEnvironment _environment, IConfiguration _configuration)//IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            Environment = _environment;
            Configuration = _configuration;
            //this.unitOfWork = unitOfWork;
        }

        [HttpPost("Check")]
        //[AllowAnonymous]
        public async Task<IActionResult> Check(string username, string password)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = Environment.WebRootPath;
                string filePath = Path.Combine(webRootPath, "UploadedFolders", "veritabani.xlsx");


                UsersModel user = ReadUsersFromExcelById(filePath, username, password);
                   
                if (user.id != 0)
                {

                    var claims = new List<Claim>
                    {
                       new Claim("id", user.id.ToString()),
                      new Claim("username", user.username.ToString()),
                      new Claim(ClaimTypes.Name, user.name.ToString()),
                      new Claim(ClaimTypes.Surname, user.surname.ToString())
                     };

                    var claimsIdentity = new ClaimsIdentity(
                              claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        //  IsPersistent = loginmodel.rememberme
                    };

                    UsersModel um = new UsersModel();
                    um.username = username;
                    um.password = int.Parse(password);
                    um.name = user.name;
                    um.surname = user.surname;
                    um.status = user.status;
                    
                    return Ok(um);
                }
                else

                return BadRequest("Lütfen bilgilerinizi kontrol ediniz.");
            }
            else
            {
                var messages = ModelState.ToList();
                return BadRequest(ModelState);
            }
        }

        [HttpGet("ReadUsersFromExcelById")]
        public UsersModel ReadUsersFromExcelById(string filePath, string username,string password)
        {
            UsersModel users = new UsersModel();

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                ISheet sheet = workbook.GetSheetAt(2); // Users çalışma sayfasını alır

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    IRow excelRow = sheet.GetRow(row);

                    string rowUsername = excelRow.GetCell(3).StringCellValue; // Satırdaki kullanıcı adını alır
                    int rowPassword = (int)excelRow.GetCell(4).NumericCellValue; // Satırdaki şifreyi alır

                    // Eğer kullanıcı adı ve şifre eşleşirse
                    if (rowUsername == username && rowPassword == int.Parse(password))
                    {

                        users = new UsersModel
                        {
                            id = (int)excelRow.GetCell(0).NumericCellValue,
                            name = excelRow.GetCell(1).StringCellValue,
                            surname = excelRow.GetCell(2).StringCellValue,
                            username = excelRow.GetCell(3).StringCellValue,
                            password = (int)excelRow.GetCell(4).NumericCellValue,
                        };
                        break; // Kullanıcıyı bulduktan sonra döngüyü kırmak için
                    }
                }
            }

            return users;
        }


    }
}
