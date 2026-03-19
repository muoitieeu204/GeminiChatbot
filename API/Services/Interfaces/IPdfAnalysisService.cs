using API.Models;

namespace API.Services.Interfaces
{
    public interface IPdfAnalysisService
    {
      Task<PdfAnalysisResponse> AnalyzePdfAsync(IFormFile file, string prompt);
  Task<bool> ValidatePdfFileAsync(IFormFile file);
    }
}
