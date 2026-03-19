namespace API.Models
{
    public class PdfDocument
    {
        public string FileName { get; set; } = string.Empty;
        public string Base64Content { get; set; } = string.Empty;
      public string TextContent { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public long FileSizeInBytes { get; set; }
    }
}
