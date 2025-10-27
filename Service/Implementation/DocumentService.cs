using System.Net.Http.Headers;
using TalkWithAyodeji.Service.Interface;
using iText.Pdfa.Checker;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Text;
using Microsoft.Extensions.FileProviders;
using TalkWithAyodeji.Service.Dto.Response;
using LangChain.Splitters.Text;
using Qdrant.Client.Grpc;
using Newtonsoft.Json;
using TalkWithAyodeji.Service.Dto.Embedding;
using TalkWithAyodeji.Service.Mapper;

namespace TalkWithAyodeji.Service.Implementation
{
    public class DocumentService : IDocumentService
    {
        private readonly ILogger<DocumentService> _logger;

        //private readonly IWebHostEnvironment _env;
        private readonly IHttpClientService _httpClient;
        private readonly IConfiguration _config;
        private readonly IEmbeddingService _embeddingService;
        private readonly IQdrantService _qdrantService;

        public DocumentService(IWebHostEnvironment env,ILogger<DocumentService> logger ,IHttpClientService httpClient, IConfiguration config, IEmbeddingService embeddingService
            , IQdrantService qdrantService)
        {
            _logger = logger;
            //_env = env;
            _httpClient = httpClient;
            _config = config;
            _embeddingService = embeddingService;
            _qdrantService = qdrantService;
        }

        public async Task<ServiceResponseDto<List<string>>> ChunkText(string text)
        {
            try
            {
                 var textSplitter = new RecursiveCharacterTextSplitter(
                chunkSize: 500,
                chunkOverlap: 150,
                separators: new[] { "\n\n", "\n", " ", "" }
                );

                var chunks = textSplitter.SplitText(text);
                return ServiceResponseDto<List<string>>.SuccessResponse("Successfully chunked text",chunks.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occured while chunking.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

                return ServiceResponseDto<List<string>>.ErrorResponse("Something unexpected happened while chunking textx", default, ex.Message);
            }
        }

        public async Task<ServiceResponseDto<List<PointStruct>>> CreateEmbeddings(List<string> texts, int dimensions)
        {
            try
            {
                //List<PointStruct> points = new List<PointStruct>();
                var embeddings = await _embeddingService.CreateEmbeddings(texts,dimensions);

                return ServiceResponseDto<List<PointStruct>>.SuccessResponse("Successfully completed chunk embeddings", embeddings.Data);

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"An unexpected error occured while embedding the chunks.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

                return ServiceResponseDto<List<PointStruct>>.ErrorResponse("An unexpected error occured while embedding the chunks", default, ex.Message);
            }
        }

        public async Task<ServiceResponseDto<string>> ExtractText(string tempPath)
        {
            try
            {
                var extractedText = new StringBuilder();
                using (var reader = new PdfReader(tempPath))
                using (var pdfDoc = new PdfDocument(reader))
                {
                    for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
                    {
                        var text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page));
                        extractedText.AppendLine(text);
                    }
                }
                // Clean up temp file
                File.Delete(tempPath);
                return ServiceResponseDto<string>.SuccessResponse("Successfully extracted text", extractedText.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Unable to extract text.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

                return ServiceResponseDto<string>.ErrorResponse("Unable to extract text", default, ex.Message);
            }
        }

        public async Task<ApiResponseDto<string>> UploadDocument(IFormFile document)
        {

            try
            {
                _logger.LogInformation("Starting Document Upload Opertaions....");
                string collectionName = "TalkWithAyodeji";
                var tempFilePath = Path.GetTempFileName();
                var originalFileName = ContentDispositionHeaderValue.Parse(document.ContentDisposition).FileName.Trim('"');
                var fileExtension = Path.GetExtension(originalFileName);
                if (fileExtension != ".pdf")
                {
                    return ApiResponseDto<string>.ErrorResponse("Only PDF files are accepted", default, originalFileName);
                }
              
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await document.CopyToAsync(stream);
                };

                _logger.LogInformation("1/5......Extracting texts from document....");
                var extractedText = await ExtractText(tempFilePath);
                if (extractedText.Success)
                {

                    _logger.LogInformation("2/5......Chunking texts....");
                    //CHUNK TEXT
                    var chunks = await ChunkText(extractedText.Data);
                    if (!chunks.Success)
                    {
                        return ApiResponseDto<string>.ErrorResponse(chunks.Message,default, chunks.Errors);
                    }

                    _logger.LogInformation("3/5......Creating Embeddings....");
                    //EMBED CHUNKS
                    var embeddings = await CreateEmbeddings(chunks.Data, 64);
                    if (!embeddings.Success)
                    {
                        return ApiResponseDto<string>.ErrorResponse(embeddings.Message, default, embeddings.Errors);
                    }

                    _logger.LogInformation("4/5......Making sure collection is intact....");
                    //DELETE COLLECTION
                    var deleteCollection = await _qdrantService.DeleteCollection(collectionName);
                    if (!deleteCollection.Success)
                    {
                        return ApiResponseDto<string>.ErrorResponse(deleteCollection.Message, default, deleteCollection.Errors);
                    }
                    //CREATE NEW COLLECTION
                    var createCollection = await _qdrantService.CreateCollection(collectionName,64);
                    if (!createCollection.Success)
                    {
                        return ApiResponseDto<string>.ErrorResponse(createCollection.Message, default, createCollection.Errors);

                    }

                    _logger.LogInformation("5/5......Storing vectors in database....");
                    //STORE CHUNKS IN VECTOR COLLECTION
                    var isStored = await _qdrantService.AddVectorsToCollection(collectionName,embeddings.Data);
                    if (!isStored.Success)
                    {
                        return ApiResponseDto<string>.ErrorResponse(isStored.Message, default, isStored.Errors);
                    }
                    return ApiResponseDto<string>.SuccessResponse("Successfully uploaded document", default);
                }
                return ApiResponseDto<string>.ErrorResponse("Unable to upload document", default, extractedText.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occured while uploading document.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

                return ApiResponseDto<string>.ErrorResponse("Unable to upload document", default, ex.Message);
            }
            
        }
    }
}
