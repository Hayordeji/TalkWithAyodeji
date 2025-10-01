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

        public BackgroundService(ApplicationDbContext context,ILogger<BackgroundService> logger, IQdrantService qdrantService)
        {
            _context = context;
            _logger = logger;
            _qdrantService = qdrantService;
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
            await Task.CompletedTask;

            _logger.LogInformation("Background job has ended");

        }

    }
}
