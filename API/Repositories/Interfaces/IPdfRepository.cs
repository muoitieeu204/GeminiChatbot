using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IPdfRepository
    {
        Task<PdfDocument> ProcessPdfFileAsync(IFormFile file);
     Task<string> ConvertPdfToBase64Async(IFormFile file);
        Task<string> ExtractTextFromPdfAsync(IFormFile file);
    }
}
