using API.Models;
using API.Repositories.Interfaces;
using iTextSharp.text.pdf;
using System.Text;
using PdfDocument = API.Models.PdfDocument;

namespace API.Repositories
{
    public class PdfRepository : IPdfRepository
    {
  private readonly ILogger<PdfRepository> _logger;

      public PdfRepository(ILogger<PdfRepository> logger)
     {
 _logger = logger;
        }

 public async Task<PdfDocument> ProcessPdfFileAsync(IFormFile file)
   {
     try
 {
    if (file == null || file.Length == 0)
      {
   throw new ArgumentException("File is empty or null");
      }

        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
       {
      throw new ArgumentException("File must be a PDF");
   }

   var pdfDocument = new PdfDocument
    {
  FileName = file.FileName,
     FileSizeInBytes = file.Length
      };

      // Extract text content
      pdfDocument.TextContent = await ExtractTextFromPdfAsync(file);

      // Convert to Base64
        file.OpenReadStream().Position = 0; // Reset stream position
pdfDocument.Base64Content = await ConvertPdfToBase64Async(file);

    // Get page count
        file.OpenReadStream().Position = 0; // Reset stream position
 using (var stream = file.OpenReadStream())
   {
    var reader = new PdfReader(stream);
     pdfDocument.PageCount = reader.NumberOfPages;
reader.Close();
    }

     _logger.LogInformation($"Successfully processed PDF: {file.FileName}, Pages: {pdfDocument.PageCount}");

       return pdfDocument;
   }
   catch (Exception ex)
   {
   _logger.LogError(ex, "Error processing PDF file");
     throw;
     }
        }

        public async Task<string> ConvertPdfToBase64Async(IFormFile file)
  {
   try
      {
    using (var memoryStream = new MemoryStream())
     {
 await file.CopyToAsync(memoryStream);
 var bytes = memoryStream.ToArray();
        return Convert.ToBase64String(bytes);
       }
      }
   catch (Exception ex)
         {
        _logger.LogError(ex, "Error converting PDF to Base64");
   throw;
         }
   }

      public async Task<string> ExtractTextFromPdfAsync(IFormFile file)
   {
 try
         {
         var text = new StringBuilder();

using (var stream = file.OpenReadStream())
    {
var reader = new PdfReader(stream);

    for (int page = 1; page <= reader.NumberOfPages; page++)
 {
        try
 {
 byte[] pageBytes = reader.GetPageContent(page);
 if (pageBytes != null && pageBytes.Length > 0)
            {
      var pageText = Encoding.UTF8.GetString(pageBytes);
     text.AppendLine(pageText);
          }
  text.AppendLine($"\n--- Page {page} ---\n");
      }
           catch (Exception ex)
     {
      _logger.LogWarning(ex, $"Could not extract text from page {page}");
    text.AppendLine($"\n--- Page {page} (extraction failed) ---\n");
         }
         }

         reader.Close();
   }

       return await Task.FromResult(text.ToString());
    }
     catch (Exception ex)
{
       _logger.LogError(ex, "Error extracting text from PDF");
  throw;
  }
   }
    }
}
