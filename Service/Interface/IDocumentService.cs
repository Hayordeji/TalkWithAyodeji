using Qdrant.Client.Grpc;
using TalkWithAyodeji.Service.Dto.Response;

namespace TalkWithAyodeji.Service.Interface
{
    public interface IDocumentService
    {
        Task<ApiResponseDto<string>> UploadDocument(IFormFile document);
        Task<ServiceResponseDto<string>> ExtractText(string filePath);
        Task<ServiceResponseDto<List<string>>> ChunkText(string text);
        Task<ServiceResponseDto<List<PointStruct>>> CreateEmbeddings(List<string> texts, int dimensions);

    }
}
