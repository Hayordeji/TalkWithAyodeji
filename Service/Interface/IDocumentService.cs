using TalkWithAyodeji.Service.Dto.Response;

namespace TalkWithAyodeji.Service.Interface
{
    public interface IDocumentService
    {
        Task<ApiResponseDto<string>> UploadDocument(IFormFile document);
        Task<ServiceResponseDto<string>> ExtractText(string filePath);
    }
}
