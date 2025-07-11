using TalkWithAyodeji.Service.Dto.Auth;
using TalkWithAyodeji.Service.Dto.Response;

namespace TalkWithAyodeji.Service.Interface
{
    public interface IAuthService
    {
        Task<ApiResponseDto<NewUserDto>> Login(LoginDto loginDto);
    }
}
