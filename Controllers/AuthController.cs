using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TalkWithAyodeji.Data.DatabaseObject;
using TalkWithAyodeji.Service.Dto.Auth;
using TalkWithAyodeji.Service.Dto.Response;
using TalkWithAyodeji.Service.Interface;

namespace TalkWithAyodeji.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;
        public AuthController(IAuthService authService, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("admin/login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.Login(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("admin/test")]
        public async Task<IActionResult> TestAdminExist()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync("MOLEFOX6@GMAIL.COM");
                if (user == null)
                {
                    return BadRequest(ApiResponseDto<string>.ErrorResponse("Test failed", default));
                }
                return Ok(ApiResponseDto<string>.SuccessResponse("Test successful","User exists"));
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An unexpected error occured :{ex.Message}");
            }

        }
    }
}
