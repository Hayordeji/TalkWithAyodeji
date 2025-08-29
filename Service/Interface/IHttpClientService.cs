namespace TalkWithAyodeji.Service.Interface
{
    public interface IHttpClientService
    {
        void SetAuthorizationHeader(string scheme, string token);
        void AddJsonHeader();

        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostAsync<T>(string url, T data);
        Task<HttpResponseMessage> PutAsync<T>(string url, T data);
        Task<HttpResponseMessage> DeleteAsync(string url);

    }
}
