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
                //var abs_folder_path = Path.Combine(_env.WebRootPath, "uploads", "admindata");
                //if (!Directory.Exists(abs_folder_path))
                //{
                //    Directory.CreateDirectory(abs_folder_path);
                //}
                // Create a temporary file path
                var tempFilePath = Path.GetTempFileName();
                var originalFileName = ContentDispositionHeaderValue.Parse(document.ContentDisposition).FileName.Trim('"');
                var fileExtension = Path.GetExtension(originalFileName);
                if (fileExtension != ".pdf")
                {
                    return ApiResponseDto<string>.ErrorResponse("Only PDF files are accepted", default, originalFileName);
                }
                //var tempPath = Path.Combine(Directory.GetCurrentDirectory(), abs_folder_path);
                //var fullPath = Path.Combine(tempFilePath, document.FileName);
                //var dbPath = Path.Combine("uploads", "admindata", document.FileName);
                //var convertedFileDbPath = "";

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
            //stream.Close();
            //throw new NotImplementedException();
        }
    }
}
