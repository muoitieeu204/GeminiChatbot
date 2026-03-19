using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IGeminiRepository
    {
        Task<string> SendPromptWithPdfAsync(string base64Pdf, string prompt);
      Task<string> SendPromptAsync(string prompt);
    }
}
