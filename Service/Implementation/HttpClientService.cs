using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using TalkWithAyodeji.Service.Interface;

namespace TalkWithAyodeji.Service.Implementation
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public HttpClientService(HttpClient httpClient, IConfiguration config)
        {
            _config = config;
            _httpClient = httpClient;
        }
        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            try
            {
                return await _httpClient.DeleteAsync(url);
            }
            catch (Exception ex) 
            {
                throw new Exception("Error in DeleteAsync", ex);
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            try
            {
                return await _httpClient.GetAsync(url);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
           
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return await _httpClient.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in PostAsync", ex);
            }
            
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return await _httpClient.PutAsync(url, content);
            }
            catch(Exception ex)
            {
                throw new Exception("Error in PutAsync", ex);
            }
        }

        //public void SetAuthorizationHeader(string token)
        //{
        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //}

        public void SetAuthorizationHeader(string scheme,string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
        }

        public void AddJsonHeader()
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        



    }
}
