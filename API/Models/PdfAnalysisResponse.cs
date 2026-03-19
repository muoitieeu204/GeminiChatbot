namespace API.Models
{
    public class PdfAnalysisResponse
    {
   public bool Success { get; set; }
    public string Answer { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public string? FileName { get; set; }
        public int PageCount { get; set; }
    }
}
