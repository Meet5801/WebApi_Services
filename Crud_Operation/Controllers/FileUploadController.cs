using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using Crud_Operation.Model;
using System;
using Crud_Operation.Model.data;
using OfficeOpenXml;

[Route("api/[controller]")]
[ApiController]
public class FileUploadController : ControllerBase
{
    private readonly UserDbContext _context;

    public FileUploadController(UserDbContext context)
    {
        _context = context;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var existingfile = await _context.UploadedFiles.FirstOrDefaultAsync(f => f.FileName == file.FileName);

        if (existingfile != null)
            return Conflict("A file with the same name already exists.");

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        var filePath = Path.Combine(folderPath, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var uploadedFile = new UploadedFile
        {
            FileName = file.FileName,
            FilePath = filePath,
            UploadedAt = DateTime.Now
        };

        _context.UploadedFiles.Add(uploadedFile);
        await _context.SaveChangesAsync();

        return Ok(new { uploadedFile.Id,uploadedFile.FileName});
    }

     [HttpPost("import")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded."); 

        var purchaseOrders = new List<PurchaseOrder>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return BadRequest("The Excel file is empty or not valid.");

                // Assuming your Excel columns are in the same order as the PurchaseOrder properties
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++) // Start from row 2 to skip header
                {
                    var purchaseOrder = new PurchaseOrder
                    {
                        Company = worksheet.Cells[row, 1].Text,
                        PoId = worksheet.Cells[row, 2].Text,
                        Reference = worksheet.Cells[row, 3].Text,
                        PoDate = DateTime.TryParse(worksheet.Cells[row, 4].Text, out var poDate) ? poDate : (DateTime?)null,
                        EventName = worksheet.Cells[row, 5].Text,
                        EventDate = DateTime.TryParse(worksheet.Cells[row, 6].Text, out var eventDate) ? eventDate : (DateTime?)null,
                        EventTime = worksheet.Cells[row, 7].Text,
                        Venue = worksheet.Cells[row, 8].Text,
                        Section = worksheet.Cells[row, 9].Text,
                        Row = worksheet.Cells[row, 10].Text,
                        Seats = worksheet.Cells[row, 11].Text,
                        Qty = int.TryParse(worksheet.Cells[row, 12].Text, out var qty) ? qty : 0,
                        Face = decimal.TryParse(worksheet.Cells[row, 13].Text, out var face) ? face : 0,
                        Cost = decimal.TryParse(worksheet.Cells[row, 14].Text, out var cost) ? cost : 0,
                        EDelivery = bool.TryParse(worksheet.Cells[row, 15].Text, out var eDelivery) && eDelivery,
                        InternalNotes = worksheet.Cells[row, 16].Text,
                        ExternalNotes = worksheet.Cells[row, 17].Text,
                        ProcurementNotes = worksheet.Cells[row, 18].Text,
                        InHand = bool.TryParse(worksheet.Cells[row, 19].Text, out var inHand) && inHand,
                        IsAvailable = bool.TryParse(worksheet.Cells[row, 20].Text, out var isAvailable) && isAvailable,
                        AvailableDate = DateTime.TryParse(worksheet.Cells[row, 21].Text, out var availableDate) ? availableDate : (DateTime?)null,
                        TrackingNumber = worksheet.Cells[row, 22].Text,
                        PurchaseStatus = worksheet.Cells[row, 23].Text,
                    };

                    purchaseOrders.Add(purchaseOrder);
                }
            }
        }

        await _context.PurchaseOrders.AddRangeAsync(purchaseOrders);
        await _context.SaveChangesAsync();

        return Ok(new { Count = purchaseOrders.Count });
    }
    
}