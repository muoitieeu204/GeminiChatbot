using API.Models;
using API.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace API.Repositories
{
    public class GeminiRepository : IGeminiRepository
    {
   private readonly GeminiSettings _settings;
   private readonly ILogger<GeminiRepository> _logger;
      private readonly HttpClient _httpClient;

public GeminiRepository(
    IOptions<GeminiSettings> settings,
     ILogger<GeminiRepository> logger,
          HttpClient httpClient)
        {
          _settings = settings.Value;
      _logger = logger;
            _httpClient = httpClient;
     }

        public async Task<string> SendPromptWithPdfAsync(string base64Pdf, string prompt)
   {
            try
  {
          _logger.LogInformation("Sending prompt to Gemini API with PDF attachment");

      var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";

    var requestBody = new
       {
    contents = new[]
       {
       new
    {
parts = new object[]
  {
 new { text = prompt },
     new
      {
      inline_data = new
        {
       mime_type = "application/pdf",
  data = base64Pdf
   }
           }
    }
                  }
             },
          generationConfig = new
     {
       temperature = _settings.Temperature,
      maxOutputTokens = _settings.MaxTokens
             }
                };

            var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

         var response = await _httpClient.PostAsync(apiUrl, content);
 var responseContent = await response.Content.ReadAsStringAsync();

  if (!response.IsSuccessStatusCode)
        {
         _logger.LogError($"Gemini API error: {response.StatusCode} - {responseContent}");
         throw new Exception($"Gemini API error: {response.StatusCode} - {responseContent}");
                }

             var jsonResponse = JsonDocument.Parse(responseContent);
  var text = jsonResponse.RootElement
              .GetProperty("candidates")[0]
      .GetProperty("content")
   .GetProperty("parts")[0]
       .GetProperty("text")
         .GetString();

           if (string.IsNullOrEmpty(text))
             {
    throw new Exception("No response text received from Gemini API");
       }

 _logger.LogInformation("Successfully received response from Gemini API");
  return text;
            }
            catch (Exception ex)
            {
    _logger.LogError(ex, "Error sending prompt to Gemini API with PDF");
     throw;
    }
        }

        public async Task<string> SendPromptAsync(string prompt)
    {
      try
            {
   _logger.LogInformation("Sending prompt to Gemini API");

       var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";

                var requestBody = new
     {
    contents = new[]
            {
             new
           {
  parts = new[]
  {
          new { text = prompt }
      }
             }
         },
         generationConfig = new
         {
          temperature = _settings.Temperature,
    maxOutputTokens = _settings.MaxTokens
       }
    };

          var json = JsonSerializer.Serialize(requestBody);
   var content = new StringContent(json, Encoding.UTF8, "application/json");

              var response = await _httpClient.PostAsync(apiUrl, content);
    var responseContent = await response.Content.ReadAsStringAsync();

           if (!response.IsSuccessStatusCode)
        {
   _logger.LogError($"Gemini API error: {response.StatusCode} - {responseContent}");
          throw new Exception($"Gemini API error: {response.StatusCode} - {responseContent}");
           }

                var jsonResponse = JsonDocument.Parse(responseContent);
     var text = jsonResponse.RootElement
         .GetProperty("candidates")[0]
       .GetProperty("content")
              .GetProperty("parts")[0]
      .GetProperty("text")
        .GetString();

    if (string.IsNullOrEmpty(text))
      {
  throw new Exception("No response text received from Gemini API");
        }

         _logger.LogInformation("Successfully received response from Gemini API");
        return text;
            }
         catch (Exception ex)
 {
  _logger.LogError(ex, "Error sending prompt to Gemini API");
    throw;
            }
        }
    }
}
