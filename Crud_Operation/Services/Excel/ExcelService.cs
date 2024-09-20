using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml; // Make sure you have the EPPlus NuGet package
using Microsoft.EntityFrameworkCore;
using Crud_Operation.Model.data;

namespace Crud_Operation.Services.Excel
{
    public class ExcelService : IExcelService
    {
        private readonly UserDbContext _context;

        public ExcelService(UserDbContext context)
        {
            _context = context;
        }

        public async Task<string> UploadExcelFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null");

            // Sanitize the table name
            var tableName = SanitizeTableName(Path.GetFileNameWithoutExtension(file.FileName));

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    // Create the SQL for table creation
                    var createTableSql = $"CREATE TABLE [{tableName}] (Id INT PRIMARY KEY IDENTITY(1,1)";

                    for (int col = 1; col <= colCount; col++)
                    {
                        var columnName = SanitizeColumnName(worksheet.Cells[1, col].Text);
                        createTableSql += $", [{columnName}] NVARCHAR(MAX)";
                    }
                    createTableSql += ");";

                    await _context.Database.ExecuteSqlRawAsync(createTableSql);

                    // Populate the table
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var values = string.Join(", ", Enumerable.Range(1, colCount).Select(col => $"'{worksheet.Cells[row, col].Text.Replace("'", "''")}'"));
                        var columnNames = string.Join(", ", Enumerable.Range(1, colCount).Select(col => $"[{SanitizeColumnName(worksheet.Cells[1, col].Text)}]"));
                        var insertSql = $"INSERT INTO [{tableName}] ({columnNames}) VALUES ({values});";
                        await _context.Database.ExecuteSqlRawAsync(insertSql);
                    }
                }
            }

            return tableName;
        }

        private string SanitizeTableName(string tableName)
        {
            // Replace invalid characters
            return tableName
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "''")
                .Trim();
        }

        private string SanitizeColumnName(string columnName)
        {
            // Replace invalid characters and trim
            return columnName
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "''")
                .Trim();
        }
    }
}
