using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services
{
    public class PdfAnalysisService : IPdfAnalysisService
    {
        private readonly IPdfRepository _pdfRepository;
        private readonly IGeminiRepository _geminiRepository;
        private readonly ILogger<PdfAnalysisService> _logger;

        // Maximum file size: 20MB
        private const long MaxFileSizeInBytes = 20 * 1024 * 1024;

        public PdfAnalysisService(
            IPdfRepository pdfRepository,
            IGeminiRepository geminiRepository,
            ILogger<PdfAnalysisService> logger)
        {
            _pdfRepository = pdfRepository;
            _geminiRepository = geminiRepository;
            _logger = logger;
        }

        public async Task<PdfAnalysisResponse> AnalyzePdfAsync(IFormFile file, string prompt)
        {
            try
            {
                // Validate file
                var isValid = await ValidatePdfFileAsync(file);
                if (!isValid)
                {
                    return new PdfAnalysisResponse
                    {
                        Success = false,
                        ErrorMessage = "Invalid PDF file"
                    };
                }

                // Process PDF
                var pdfDocument = await _pdfRepository.ProcessPdfFileAsync(file);

                // Check if question is related to the document's topic area
                var isRelevant = await CheckQuestionRelevanceAsync(prompt, pdfDocument.TextContent);

                if (!isRelevant)
                {
                    return new PdfAnalysisResponse
                    {
                        Success = false,
                        Answer = "I can only answer questions related to the topics covered in the uploaded PDF document. Your question appears to be completely unrelated to this document's subject matter.",
                        ErrorMessage = "Question not relevant to document",
                        FileName = pdfDocument.FileName,
                        PageCount = pdfDocument.PageCount
                    };
                }

                // Create enhanced prompt with context awareness
                var enhancedPrompt = CreateEnhancedPrompt(prompt, pdfDocument.TextContent);

                // Send to Gemini API
                var answer = await _geminiRepository.SendPromptWithPdfAsync(
                    pdfDocument.Base64Content,
                    enhancedPrompt);

                return new PdfAnalysisResponse
                {
                    Success = true,
                    Answer = answer,
                    FileName = pdfDocument.FileName,
                    PageCount = pdfDocument.PageCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing PDF");
                return new PdfAnalysisResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<bool> ValidatePdfFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("File is null or empty");
                return false;
            }

            if (file.Length > MaxFileSizeInBytes)
            {
                _logger.LogWarning($"File size exceeds maximum allowed size: {file.Length} bytes");
                return false;
            }

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning($"File is not a PDF: {file.FileName}");
                return false;
            }

            return await Task.FromResult(true);
        }

        private string CreateEnhancedPrompt(string userPrompt, string documentText)
        {
            return $@"You are an AI assistant analyzing a PDF document. Your role is to answer questions about the document's content and related technical topics.

INSTRUCTIONS:
1. **Primary Source**: Use the document content as your primary source of information.
2. **Comparisons Allowed**: If the question asks to compare or contrast the document's topics with related concepts (e.g., ""Compare gRPC with REST"" when document is about gRPC), you MUST answer using BOTH the document AND your general knowledge.
3. **Related Technical Topics**: For questions about advantages, disadvantages, use cases, or technical analysis of the document's topics, provide comprehensive answers using document content + general technical knowledge.
4. **Be Clear**: When answering, mention what comes from the document vs. what is general knowledge.
5. **Decline Only Unrelated**: Only decline questions completely unrelated to the document's domain (e.g., weather, sports, celebrities).

Document Content:
{(documentText.Length > 3000 ? documentText.Substring(0, 3000) + "..." : documentText)}

User Question: {userPrompt}

Provide a comprehensive answer. For comparisons and technical analysis, use both document content and your knowledge. Only decline if the question has no connection to the document's topic.";
        }

        private async Task<bool> CheckQuestionRelevanceAsync(string prompt, string documentText)
        {
            try
            {
                // Use Gemini to check if the question is relevant to the document's topic area
                var relevanceCheckPrompt = $@"Determine if the question is relevant to the document's TOPIC AREA (not just exact content).

Question: {prompt}

Document Content Preview:
{(documentText.Length > 2000 ? documentText.Substring(0, 2000) + "..." : documentText)}

**CRITERIA FOR 'YES':**
- Question is about document content, OR
- Question compares document topics with related concepts (e.g., gRPC vs REST when doc is about gRPC), OR  
- Question asks about advantages/disadvantages/use cases of document topics, OR
- Question seeks technical explanation related to document's subject matter

**CRITERIA FOR 'NO':**
- Question is about completely unrelated domains (weather, sports, celebrities, cooking when doc is technical)

**EXAMPLES:**
✅ Doc: gRPC | Q: ""Compare gRPC with REST"" = YES
✅ Doc: Python | Q: ""Python vs Java?"" = YES
✅ Doc: React | Q: ""When use React vs Angular?"" = YES
✅ Doc: SQL | Q: ""SQL vs NoSQL differences"" = YES
❌ Doc: Technology | Q: ""What's the weather?"" = NO
❌ Doc: Programming | Q: ""Who won the Super Bowl?"" = NO

Reply ONLY 'YES' or 'NO':";

                var response = await _geminiRepository.SendPromptAsync(relevanceCheckPrompt);
                var cleanResponse = response.Trim().ToUpper();

                _logger.LogInformation($"Relevance Check | Question: '{prompt}' | Result: '{cleanResponse}'");

                // Check if response contains YES
                return cleanResponse.Contains("YES");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error checking question relevance, allowing question to proceed");
                // If relevance check fails, allow the question (fail open)
                return true;
            }
        }
    }
}
