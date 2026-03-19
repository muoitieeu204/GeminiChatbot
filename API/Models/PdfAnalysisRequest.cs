namespace API.Models
{
    public class PdfAnalysisRequest
    {
        public IFormFile File { get; set; } = null!;
        public string Prompt { get; set; } = string.Empty;
    }
}
