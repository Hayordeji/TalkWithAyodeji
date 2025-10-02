using Hangfire;
using Microsoft.EntityFrameworkCore;
using TalkWithAyodeji.Repository.Data;
using TalkWithAyodeji.Service.Interface;

namespace TalkWithAyodeji.Service.Implementation
{
    public class BackgroundService : IBackgroundService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BackgroundService> _logger;
        private readonly IQdrantService _qdrantService;
        private readonly IHttpClientService _httpClientService;

        public BackgroundService(ApplicationDbContext context, IHttpClientService httpClientService, ILogger<BackgroundService> logger, IQdrantService qdrantService)
        {
            _context = context;
            _logger = logger;
            _qdrantService = qdrantService;
            _httpClientService = httpClientService;
        }
        public async Task KeepServerActive()
        {
            RecurringJob.AddOrUpdate<BackgroundService>(
                  recurringJobId: $"Keep Server Active",
                  methodCall: service => service.KeepActivity(),
                  cronExpression: "0/14 * * * *",
                  options: new RecurringJobOptions
                  {
                      TimeZone = TimeZoneInfo.Utc,
                  }
            );
            await Task.CompletedTask;


        }


        public async Task KeepActivity()
        {
            _logger.LogInformation("Background job has started");

            bool isExist = await _context.Users.AnyAsync();
            await _qdrantService.CreateCollection("Test", 1);
            await _qdrantService.DeleteCollection("Test");

            //PING THE API
            await _httpClientService.GetAsync("https://talkwithayodeji.onrender.com/api/admin/keep-alive");

            await Task.CompletedTask;

            _logger.LogInformation("Background job has ended");

        }

    }
}
