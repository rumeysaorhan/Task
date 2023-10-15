using Microsoft.AspNetCore.Hosting;
using System.Data;
using System.Data.OleDb;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Row = DocumentFormat.OpenXml.Spreadsheet.Row;

namespace MepasTask.API.Helpers
{
    public class ReadWriteData
    {
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;

        public ReadWriteData(IWebHostEnvironment _environment, IConfiguration _configuration)
        {
            Environment = _environment;
            Configuration = _configuration;
        }

        public DataTable ReadExcelToDataTable(string folderName, string fileName)
        {
            string fileWithPath = Path.Combine(this.Environment.WebRootPath, folderName, fileName);
            DataTable dataTable = new DataTable();

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fileWithPath, false))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                SharedStringTablePart sharedStringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();

                if (worksheetPart != null)
                {
                    Worksheet worksheet = worksheetPart.Worksheet;
                    SheetData sheetData = worksheet.GetFirstChild<SheetData>();

                    // Read the first row to determine column headers
                    Row headerRow = sheetData.Elements<Row>().FirstOrDefault();
                    if (headerRow != null)
                    {
                        foreach (Cell cell in headerRow.Elements<Cell>())
                        {
                            string columnHeader = GetCellValue(cell, sharedStringTablePart);
                            dataTable.Columns.Add(columnHeader);
                        }
                    }

                    // Read the data rows and populate the DataTable
                    foreach (Row row in sheetData.Elements<Row>().Skip(1)) // Skip the header row
                    {
                        DataRow dataRow = dataTable.NewRow();
                        int columnIndex = 0;

                        foreach (Cell cell in row.Elements<Cell>())
                        {
                            string cellValue = GetCellValue(cell, sharedStringTablePart);
                            dataRow[columnIndex] = cellValue;
                            columnIndex++;
                        }

                        dataTable.Rows.Add(dataRow);
                    }
                }
            }

            return dataTable;
        }

      
        private static string GetCellValue(Cell cell, SharedStringTablePart sharedStringTablePart)
        {
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                int sharedStringIndex = int.Parse(cell.InnerText);
                SharedStringItem item = sharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(sharedStringIndex);
                return item.Text.Text;
            }
            else if (cell.CellValue != null)
            {
                return cell.CellValue.Text;
            }
            else
            {
                return null;
            }
        }

      
        public async Task<string> SaveFileToFolder(IFormFile postedFile, string folderName, string filename)
        {
            string path = Path.Combine(this.Environment.WebRootPath, folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            FileInfo fileInfo = new FileInfo(postedFile.FileName);
            string fileName = Path.GetFileNameWithoutExtension(filename) + Path.GetExtension(fileInfo.Extension);
            string filePath = Path.Combine(path, fileName);

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                try
                {
                    postedFile.CopyTo(stream);
                    return fileName;
                }
                catch (Exception ex)
                {
                    return "error";
                }
            }

        }

        public async Task<string> DeleteFileToFolder(string folderName, string filename)
        {
            string path = Path.Combine(this.Environment.WebRootPath, folderName);
            string ExitingFile = Path.Combine(this.Environment.WebRootPath, folderName, filename);
            try
            {
                System.IO.File.Delete(ExitingFile);
                return "success";
            }
            catch
            {
                return "error";
            }
        }


    }
}

