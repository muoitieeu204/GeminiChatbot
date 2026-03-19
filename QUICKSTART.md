# Quick Start Guide - PDF Analysis API

## ?? Quick Setup (5 Minutes)

### Step 1: Get Your Gemini API Key
1. Visit: https://makersuite.google.com/app/apikey
2. Click "Create API Key"
3. Copy your API key

### Step 2: Configure the API
1. Open `API/appsettings.json`
2. Replace `YOUR_GEMINI_API_KEY_HERE` with your actual API key:

```json
{
  "GeminiSettings": {
    "ApiKey": "AIzaSy...", // Paste your key here
"Model": "gemini-2.0-flash-exp",
    "Temperature": 0.7,
    "MaxTokens": 8192
  }
}
```

### Step 3: Run the API
```bash
cd API
dotnet run
```

### Step 4: Test It!

**Option A - Using Swagger (Recommended)**
1. Open browser: `https://localhost:7xxx/swagger`
2. Click on `POST /api/PdfAnalysis/analyze`
3. Click "Try it out"
4. Upload a PDF and enter your question
5. Click "Execute"

**Option B - Using the HTML Test Page**
1. Open `Examples/TestPage.html` in a browser
2. Update the API_URL in the HTML file to match your server URL
3. Upload a PDF and ask questions!

**Option C - Using cURL**
```bash
curl -X POST "https://localhost:7xxx/api/PdfAnalysis/analyze" \
  -H "accept: application/json" \
  -F "file=@document.pdf" \
  -F "prompt=What is this document about?"
```

**Option D - Using PowerShell**
```powershell
$form = @{
    file = Get-Item "C:\path\to\document.pdf"
    prompt = "What is this document about?"
}
Invoke-RestMethod -Uri "https://localhost:7xxx/api/PdfAnalysis/analyze" -Method Post -Form $form
```

## ?? Example Questions to Try

**Good Questions (Will Work):**
- "What is the main topic of this document?"
- "Summarize the key points"
- "What recommendations are provided?"
- "Explain the methodology used"
- "List the main findings"

**Bad Questions (Will Be Filtered):**
- "What's the weather today?" ?
- "Tell me a joke" ?
- "Who is the president?" ?
- "Calculate 2+2" ?

## ??? Project Structure

```
GeminiChatbot/
??? API/
?   ??? Controllers/
?   ?   ??? PdfAnalysisController.cs    # API endpoints
?   ??? Services/
?   ?   ??? Interfaces/
?   ?   ?   ??? IPdfAnalysisService.cs
?   ?   ??? PdfAnalysisService.cs       # Business logic
?   ??? Repositories/
?   ?   ??? Interfaces/
?   ?   ?   ??? IPdfRepository.cs
?   ?   ?   ??? IGeminiRepository.cs
?   ?   ??? PdfRepository.cs   # PDF processing
?   ?   ??? GeminiRepository.cs         # Gemini API calls
?   ??? Models/
? ?   ??? PdfDocument.cs
?   ?   ??? PdfAnalysisRequest.cs
?   ?   ??? PdfAnalysisResponse.cs
?   ?   ??? GeminiSettings.cs
?   ??? Program.cs          # DI Configuration
?   ??? appsettings.json      # Configuration
??? Examples/
?   ??? ClientExample.cs       # C# client example
? ??? TestPage.html  # Web test page
??? README.md         # Full documentation
```

## ?? Troubleshooting

### "API Key Error"
- Check your API key is correct in `appsettings.json`
- Ensure there are no extra spaces or quotes
- Verify key is active at https://makersuite.google.com/app/apikey

### "Question Not Relevant"
This is normal! The API filters out questions not related to the document.
Ask questions specifically about the PDF content.

### "Build Failed"
```bash
cd API
dotnet restore
dotnet build
```

### "Port Already in Use"
Edit `API/Properties/launchSettings.json` and change the port numbers.

## ?? Key Features

? **Smart Question Filtering** - Like NotebookLM, filters out irrelevant questions  
? **Multi-Page PDF Support** - Handles PDFs with multiple pages  
? **Base64 Encoding** - Works with Gemini free tier (no file API needed)  
? **Text Extraction** - Extracts text from PDFs for context  
? **N-Layer Architecture** - Clean, maintainable code structure  
? **Swagger Documentation** - Interactive API testing  
? **File Validation** - Validates file type and size (20MB max)  

## ?? API Response Format

**Success:**
```json
{
  "success": true,
  "answer": "This document discusses artificial intelligence...",
  "errorMessage": null,
  "fileName": "ai-report.pdf",
  "pageCount": 15
}
```

**Filtered (Question Not Relevant):**
```json
{
  "success": false,
  "answer": "I can only answer questions related to the content...",
  "errorMessage": "Question not relevant to document",
  "fileName": "ai-report.pdf",
  "pageCount": 15
}
```

## ?? Security Best Practices

1. **Never commit API keys** to version control
2. Use **environment variables** in production:
   ```bash
   export GeminiSettings__ApiKey="your-key-here"
   ```
3. Add **authentication** to your endpoints
4. Implement **rate limiting**
5. Use **HTTPS** in production

## ?? Required Packages

All packages are already configured:
- `Mscc.GenerativeAI` - Gemini API SDK
- `iTextSharp.LGPLv2.Core` - PDF text extraction
- `Swashbuckle.AspNetCore` - Swagger documentation

## ?? Production Deployment

### Azure App Service
```bash
az webapp up --name your-app-name --runtime "DOTNET:8.0"
az webapp config appsettings set --name your-app-name --settings GeminiSettings__ApiKey="your-key"
```

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["dotnet", "API.dll"]
```

## ?? Tips

1. **Test with small PDFs first** - Start with 1-2 page documents
2. **Be specific with questions** - Reference specific sections or topics
3. **Check the logs** - Console output shows detailed processing steps
4. **Use Swagger** - Best way to test the API during development
5. **Monitor API usage** - Free tier has rate limits (60 req/min)

## ?? Getting Help

1. Check the full **README.md** for detailed documentation
2. Review console logs for error details
3. Test with Swagger UI for debugging
4. Verify your Gemini API key is active
5. Ensure PDF is not password-protected

## ?? Learn More

- Full Documentation: See `README.md`
- Gemini API Docs: https://ai.google.dev/docs
- ASP.NET Core: https://docs.microsoft.com/aspnet/core

---

**Ready to analyze your first PDF? Run `dotnet run` in the API folder and visit Swagger!** ??
