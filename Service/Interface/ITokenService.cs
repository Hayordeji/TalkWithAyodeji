using TalkWithAyodeji.Data.DatabaseObject;

namespace TalkWithAyodeji.Service.Interface
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
