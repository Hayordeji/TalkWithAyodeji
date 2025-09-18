using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<AppUser> _userManager;
        public AuthController(IAuthService authService, ILogger<AuthController> logger ,UserManager<AppUser> userManager)
        {
            _authService = authService;
            _logger = logger;
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
            _logger.LogError($"API was unable to login. Error Message : {result.Message}, Error :{result.Errors}");

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
                    _logger.LogError($"API was unable to find admin user.");
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
