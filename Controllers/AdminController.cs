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
        public AdminController(IDocumentService documentService)
        {
            _documentService = documentService;
        }


        [HttpPost("/upload")]
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
    }
}
