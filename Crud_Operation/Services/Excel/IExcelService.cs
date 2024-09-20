namespace Crud_Operation.Services.Excel
{
    public interface IExcelService
    {
        Task<string> UploadExcelFileAsync(IFormFile file);
    }
}
