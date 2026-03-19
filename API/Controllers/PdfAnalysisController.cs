using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfAnalysisController : ControllerBase
    {
        private readonly IPdfAnalysisService _pdfAnalysisService;
        private readonly ILogger<PdfAnalysisController> _logger;

        public PdfAnalysisController(
            IPdfAnalysisService pdfAnalysisService,
       ILogger<PdfAnalysisController> logger)
        {
      _pdfAnalysisService = pdfAnalysisService;
   _logger = logger;
        }

        /// <summary>
        /// Analyzes a PDF document and answers questions about its content
        /// </summary>
        /// <param name="request">The PDF analysis request containing file and prompt</param>
    /// <returns>Analysis response with answer</returns>
        [HttpPost("analyze")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(PdfAnalysisResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
      [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PdfAnalysisResponse>> AnalyzePdf([FromForm] PdfAnalysisRequest request)
        {
  try
    {
  _logger.LogInformation($"Received PDF analysis request: {request.File?.FileName}");

         if (request.File == null || string.IsNullOrWhiteSpace(request.Prompt))
        {
 return BadRequest(new PdfAnalysisResponse
      {
  Success = false,
     ErrorMessage = "Both PDF file and prompt are required"
            });
  }

 var result = await _pdfAnalysisService.AnalyzePdfAsync(request.File, request.Prompt);

   if (!result.Success)
    {
       return BadRequest(result);
       }

                return Ok(result);
            }
            catch (Exception ex)
          {
       _logger.LogError(ex, "Error in AnalyzePdf endpoint");
             return StatusCode(500, new PdfAnalysisResponse
     {
            Success = false,
        ErrorMessage = "An error occurred while processing your request"
          });
     }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
      [HttpGet("health")]
        public IActionResult HealthCheck()
        {
      return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
        }
    }
}
