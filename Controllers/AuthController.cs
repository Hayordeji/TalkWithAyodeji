using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TalkWithAyodeji.Service.Dto.Auth;
using TalkWithAyodeji.Service.Interface;

namespace TalkWithAyodeji.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
    }
}
    