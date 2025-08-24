using System.Net.Http.Headers;
using TalkWithAyodeji.Service.Interface;
using iText.Pdfa.Checker;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Text;
using Microsoft.Extensions.FileProviders;
using TalkWithAyodeji.Service.Dto.Response;

namespace TalkWithAyodeji.Service.Implementation
{
    public class DocumentService : IDocumentService
    {
        private readonly IWebHostEnvironment _env;
        
        public DocumentService(IWebHostEnvironment env)
        {
            _env = env;
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
                return ServiceResponseDto<string>.ErrorResponse("Unable to extract text", default, ex.Message);
            }
        }

        public async Task<ApiResponseDto<string>> UploadDocument(IFormFile document)
        {

            try
            {  
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
                var extractedText = await ExtractText(tempFilePath);
                if (extractedText.Success)
                {
                    return ApiResponseDto<string>.SuccessResponse("Successfully uploaded document", "Don't worry, it worked");
                }
                return ApiResponseDto<string>.ErrorResponse("Unable to upload document", default, extractedText.Message);
            }
            catch (Exception ex)
            {
                return ApiResponseDto<string>.ErrorResponse("Unable to upload document", default, ex.Message);
            }
            
        }
    }
}
