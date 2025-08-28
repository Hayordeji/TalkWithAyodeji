namespace TalkWithAyodeji.Service.Interface
{
    public interface IRedisService
    {
        Task<T?> GetData<T>(string key);
        Task SetData<T>(string key, T value);
    }
}
