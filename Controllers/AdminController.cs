using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using TalkWithAyodeji.Service.Dto.Response;
using TalkWithAyodeji.Service.Interface;

namespace TalkWithAyodeji.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<AdminController> _logger;
        private readonly IAIService _aiService;
        public AdminController(IDocumentService documentService, ILogger<AdminController> logger,IAIService aiService)
        {
            _documentService = documentService;
            _logger = logger;
            _aiService = aiService;
        }






        [HttpPost("upload-document")]
        public async Task<IActionResult> UploadDocument(IFormFile document)
        {
            //if (!ModelState.IsValid )
            //{
            //    return BadRequest(ModelState);
            //}
            var result =  await _documentService.UploadDocument(document);
            if (!result.Success)
            {
                _logger.LogError($"API was unable to Upload document. Error Message : {result.Message}, Error :{result.Errors}");
                return BadRequest(ApiResponseDto<string>.ErrorResponse(result.Message,default, result.Errors));
            }
            return Ok(result);
        }


        [HttpGet("ask-ai")]
        [Authorize]
        public async Task<IActionResult> TestAI([FromQuery] string question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _aiService.AskAI(question, "test connectionId");
            if (!result.Success)
            {
                _logger.LogError($"API was unable to ASK AI Question. Error Message : {result.Message}, Error :{result.Errors}");
                return BadRequest(ApiResponseDto<string>.ErrorResponse(result.Message, default, result.Errors));
            }
            return Ok(result);
        }
    }
}
