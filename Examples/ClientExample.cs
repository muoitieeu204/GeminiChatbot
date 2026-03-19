// Example: How to use the PDF Analysis API from a C# client

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PdfAnalysisClient
{
    class Program
 {
        static async Task Main(string[] args)
  {
    var apiUrl = "https://localhost:7xxx/api/PdfAnalysis/analyze"; // Replace with your actual URL
            var pdfFilePath = @"C:\path\to\your\document.pdf";
 var question = "What is the main topic of this document?";

            await AnalyzePdfAsync(apiUrl, pdfFilePath, question);
        }

        static async Task AnalyzePdfAsync(string apiUrl, string filePath, string prompt)
     {
        try
            {
      using var httpClient = new HttpClient();
           using var form = new MultipartFormDataContent();

    // Add PDF file
    var fileStream = File.OpenRead(filePath);
         var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
     form.Add(fileContent, "file", Path.GetFileName(filePath));

     // Add prompt
           form.Add(new StringContent(prompt), "prompt");

                // Send request
                Console.WriteLine("Sending request to API...");
        var response = await httpClient.PostAsync(apiUrl, form);
     var responseContent = await response.Content.ReadAsStringAsync();

          if (response.IsSuccessStatusCode)
     {
    Console.WriteLine("\nSuccess!");
  Console.WriteLine(responseContent);
 }
        else
    {
       Console.WriteLine("\nError!");
         Console.WriteLine($"Status Code: {response.StatusCode}");
          Console.WriteLine(responseContent);
      }
  }
        catch (Exception ex)
         {
    Console.WriteLine($"Exception: {ex.Message}");
   }
        }
    }
}

/* Expected Response Format:
{
  "success": true,
  "answer": "The main topic of this document is...",
  "errorMessage": null,
  "fileName": "document.pdf",
  "pageCount": 10
}
*/
