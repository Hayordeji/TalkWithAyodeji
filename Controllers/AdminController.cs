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
        private readonly IAIService _aiService;
        public AdminController(IDocumentService documentService, IAIService aiService)
        {
            _documentService = documentService;
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
                return BadRequest(ApiResponseDto<string>.ErrorResponse(result.Message, default, result.Errors));
            }
            return Ok(result);
        }
    }
}
