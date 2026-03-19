# PDF Analysis API with Google Gemini 2.0 Flash

This ASP.NET Core Web API allows you to upload PDF documents and ask questions about their content using Google Gemini 2.0 Flash API (Free Tier). The API includes intelligent question filtering to ensure questions are related to the document content, similar to NotebookLM.

## Architecture

The solution follows N-Layer Architecture pattern:

```
API/
??? Models/              # Data models and DTOs
?   ??? PdfDocument.cs
?   ??? PdfAnalysisRequest.cs
?   ??? PdfAnalysisResponse.cs
?   ??? GeminiSettings.cs
??? Repositories/    # Data access layer
?   ??? Interfaces/
?   ?   ??? IPdfRepository.cs
?   ?   ??? IGeminiRepository.cs
???? PdfRepository.cs
?   ??? GeminiRepository.cs
??? Services/     # Business logic layer
?   ??? Interfaces/
?   ?   ??? IPdfAnalysisService.cs
?   ??? PdfAnalysisService.cs
??? Controllers/    # API endpoints
    ??? PdfAnalysisController.cs
```

## Features

? **PDF Text Extraction** - Extracts text content from PDF documents  
? **Base64 PDF Encoding** - Converts PDF to Base64 for API transmission  
? **Question Relevance Check** - Filters out questions not related to document content (like "What's the weather today?")  
? **Context-Aware Answers** - Provides answers strictly based on document content  
? **File Validation** - Validates file type, size (max 20MB), and format  
? **Swagger Documentation** - Interactive API documentation  
? **N-Layer Architecture** - Clean separation of concerns  

## Prerequisites

- .NET 8 SDK
- Google Gemini API Key (Free Tier) - Get it from [Google AI Studio](https://makersuite.google.com/app/apikey)
- Visual Studio 2022 or VS Code

## Setup Instructions

### 1. Get Your Gemini API Key

1. Go to [Google AI Studio](https://makersuite.google.com/app/apikey)
2. Click "Create API Key"
3. Copy your API key

### 2. Configure API Key

Update the `appsettings.json` file with your Gemini API key:

```json
{
  "GeminiSettings": {
    "ApiKey": "YOUR_GEMINI_API_KEY_HERE",
    "Model": "gemini-2.0-flash-exp",
    "Temperature": 0.7,
    "MaxTokens": 8192
  }
}
```

### 3. Install Dependencies

The following packages are already configured:
- `Mscc.GenerativeAI` - Gemini API client (uses HttpClient internally)
- `iTextSharp.LGPLv2.Core` - PDF text extraction
- `Swashbuckle.AspNetCore` - Swagger documentation

### 4. Run the Application

```bash
cd API
dotnet run
```

The API will start at `https://localhost:7xxx` (or `http://localhost:5xxx`)

## API Endpoints

### 1. Analyze PDF Document

**Endpoint:** `POST /api/PdfAnalysis/analyze`

**Description:** Upload a PDF and ask questions about its content

**Request:**
- Content-Type: `multipart/form-data`
- Form Data:
  - `file`: PDF file (max 20MB)
  - `prompt`: Your question about the PDF

**Example using cURL:**

```bash
curl -X POST "https://localhost:7xxx/api/PdfAnalysis/analyze" \
  -H "accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@/path/to/your/document.pdf" \
  -F "prompt=What is the main topic of this document?"
```

**Example using PowerShell:**

```powershell
$uri = "https://localhost:7xxx/api/PdfAnalysis/analyze"
$filePath = "C:\path\to\your\document.pdf"
$prompt = "What is the main topic of this document?"

$form = @{
    file = Get-Item -Path $filePath
    prompt = $prompt
}

Invoke-RestMethod -Uri $uri -Method Post -Form $form
```

**Success Response (200 OK):**

```json
{
  "success": true,
  "answer": "The main topic of this document is...",
  "errorMessage": null,
  "fileName": "document.pdf",
"pageCount": 10
}
```

**Filtered Response (400 Bad Request) - Question not related to document:**

```json
{
  "success": false,
  "answer": "I can only answer questions related to the content of the uploaded PDF document. Your question appears to be outside the scope of this document.",
  "errorMessage": "Question not relevant to document",
  "fileName": "document.pdf",
"pageCount": 10
}
```

**Error Response (400 Bad Request):**

```json
{
  "success": false,
  "answer": "",
  "errorMessage": "Both PDF file and prompt are required",
  "fileName": null,
  "pageCount": 0
}
```

### 2. Health Check

**Endpoint:** `GET /api/PdfAnalysis/health`

**Response:**

```json
{
  "status": "Healthy",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

## Testing with Swagger

1. Run the application
2. Navigate to `https://localhost:7xxx/swagger`
3. Click on the `POST /api/PdfAnalysis/analyze` endpoint
4. Click "Try it out"
5. Upload a PDF file and enter your question
6. Click "Execute"

## Example Questions

### Valid Questions (Related to Document):
- "What is the main topic of this document?"
- "Summarize the key points from page 3"
- "What does the author say about X?"
- "List all the recommendations in this report"
- "Explain the methodology described in this paper"

### Invalid Questions (Will be Filtered):
- "What's the weather today?"
- "Who won the Super Bowl?"
- "Tell me a joke"
- "What time is it?"
- "Calculate 2+2"

## How It Works

### 1. **Question Relevance Check**

The service includes a smart filtering mechanism:

```csharp
private async Task<bool> CheckQuestionRelevanceAsync(string prompt, string documentText)
{
    // Uses Gemini to determine if question relates to document
    // Returns true if relevant, false otherwise
}
```

This ensures that only document-related questions get processed, similar to NotebookLM.

### 2. **Enhanced Prompt Construction**

The service creates an enhanced prompt with clear instructions:

```csharp
private string CreateEnhancedPrompt(string userPrompt, string documentText)
{
    // Combines user question with document context
    // Includes strict rules to stay within document scope
}
```

### 3. **PDF Processing Pipeline**

1. **Validate** - Check file type, size, and format
2. **Extract Text** - Extract text content from all pages
3. **Convert to Base64** - Encode PDF for API transmission
4. **Check Relevance** - Verify question relates to document
5. **Send to Gemini** - Process with Gemini 2.0 Flash
6. **Return Answer** - Provide document-based response

## File Size Limits

- Maximum PDF size: **20 MB**
- Can be adjusted in `Program.cs`:

```csharp
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 20 * 1024 * 1024; // 20 MB
});
```

## Configuration Options

Edit `appsettings.json` to customize:

```json
{
  "GeminiSettings": {
    "ApiKey": "your-api-key",
    "Model": "gemini-2.0-flash-exp",    // Gemini model to use
    "Temperature": 0.7,           // 0.0 - 1.0 (creativity level)
    "MaxTokens": 8192  // Maximum response length
  }
}
```

## Troubleshooting

### Issue: "Unable to find package Mscc.GenerativeAI"
**Solution:** The package has been installed. If you need to reinstall:
```bash
dotnet add package Mscc.GenerativeAI
```

### Issue: "Gemini API error: 400"
**Solution:** 
- Check your API key is valid
- Ensure PDF is not corrupted
- Verify file size is under 20MB

### Issue: "Question not relevant to document"
**Solution:** 
- Ask questions specifically about the document content
- Reference specific sections, pages, or topics from the PDF
- Avoid general knowledge questions

### Issue: PDF text extraction fails
**Solution:**
- Ensure PDF is not password-protected
- Check if PDF contains actual text (not just scanned images)
- For image-based PDFs, you may need OCR preprocessing

## API Rate Limits (Free Tier)

Google Gemini Free Tier limits:
- **60 requests per minute**
- **1,500 requests per day**

The API will throw errors if limits are exceeded.

## Development Notes

### Adding New Features

To add new PDF processing features:

1. **Add to Repository Layer** (`IPdfRepository` & `PdfRepository`)
2. **Update Service Layer** (`IPdfAnalysisService` & `PdfAnalysisService`)
3. **Expose in Controller** (`PdfAnalysisController`)

### Database Integration (Future Enhancement)

To store analysis history:

1. Create `Data` folder with DbContext
2. Add Entity Framework Core packages
3. Update repositories to use database
4. Add migration commands

## License

This project uses:
- **iTextSharp.LGPLv2.Core** - LGPL v2 License
- **Mscc.GenerativeAI** - MIT License

## Support

For issues or questions:
1. Check the troubleshooting section
2. Review Swagger documentation at `/swagger`
3. Check logs in the console output
4. Verify your Gemini API key is valid

## Security Notes

?? **Important Security Considerations:**

1. **Never commit your API key** to version control
2. Use **User Secrets** for development:
   ```bash
   dotnet user-secrets set "GeminiSettings:ApiKey" "your-api-key"
   ```
3. Use **Environment Variables** in production
4. Consider adding **authentication** to your API endpoints
5. Implement **rate limiting** to prevent abuse

## Next Steps

- [ ] Add user authentication (JWT tokens)
- [ ] Implement caching for repeated questions
- [ ] Add support for multiple file formats (DOCX, TXT, etc.)
- [ ] Store analysis history in database
- [ ] Add OCR support for image-based PDFs
- [ ] Implement batch processing for multiple files
- [ ] Add chat history to maintain conversation context

## Example Test Case

Create a simple test PDF with this content:

```
Test Document

This is a test document about artificial intelligence.
AI is transforming various industries.

Key Points:
1. Machine Learning
2. Natural Language Processing
3. Computer Vision
```

Valid Test Questions:
- "What are the three key points mentioned?"
- "What is this document about?"
- "List the AI topics covered"

Invalid Test Questions (will be filtered):
- "What's the capital of France?"
- "Tell me about quantum physics"
- "What's the weather today?"

---

**Built with ?? using ASP.NET Core 8 and Google Gemini 2.0 Flash**
