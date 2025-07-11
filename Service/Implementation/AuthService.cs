using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TalkWithAyodeji.Data.DatabaseObject;
using TalkWithAyodeji.Service.Dto.Auth;
using TalkWithAyodeji.Service.Dto.Response;
using TalkWithAyodeji.Service.Interface;

namespace TalkWithAyodeji.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<ApiResponseDto<NewUserDto>> Login(LoginDto loginDto)
        {
            try
            {
                //Check of username and pssword field are not empty
                if (string.IsNullOrWhiteSpace(loginDto.UserName)|| string.IsNullOrWhiteSpace(loginDto.UserName))
                {
                    return ApiResponseDto<NewUserDto>.ErrorResponse("Username or Password can not be null", default, default);
                }
                //Fetch the user
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == loginDto.UserName.ToUpper());

                if (user == null)
                {
                    return ApiResponseDto<NewUserDto>.ErrorResponse("User not found...",default,default);
                }
                var result = _signInManager.CheckPasswordSignInAsync(user, loginDto.Password,false);
                if (!result.Result.Succeeded)
                {
                    return ApiResponseDto<NewUserDto>.ErrorResponse("Invalid username or password", default, default);
                }
                return ApiResponseDto<NewUserDto>.SuccessResponse("Login successful", new NewUserDto
                {
                    UserName = user.UserName,
                    Token = await _tokenService.CreateToken(user),
                });
            }
            catch(Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while processing the login request.", ex);
            }
        }
    }
}
