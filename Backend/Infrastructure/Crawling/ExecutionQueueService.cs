using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Crawling
{
    public class ExecutionQueueService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExecutionQueueService> _logger;

        public ExecutionQueueService(
            IServiceProvider serviceProvider,
            ILogger<ExecutionQueueService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                   tawait ScheduleExecutionsAsync();
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Check every minute
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while scheduling executions");
                }
            }
        }

        private async Task ScheduleExecutionsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var websiteRecordRepository = scope.ServiceProvider.GetRequiredService<IWebsiteRecordRepository>();
            var executionRepository = scope.ServiceProvider.GetRequiredService<IExecutionRepository>();
            var crawler = scope.ServiceProvider.GetRequiredService<ICrawler>();

            var activeRecords = await websiteRecordRepository.GetActiveRecordsAsync();
            var tasks = activeRecords.Select(async record =>
            {
                var lastExecution = await executionRepository.GetLastExecutionFromWebsiteRecord(record.Id);
                if (lastExecution == null || ShouldStartNewExecution(lastExecution, record.Periodicity))
                {
                    await StartExecutionAsync(record, executionRepository, crawler);
                }
            });
            await Task.WhenAll(tasks);
        }

        private bool ShouldStartNewExecution(Execution lastExecution, int periodicity)
        {
            var timeSinceLastExecution = DateTime.UtcNow - lastExecution.EndTime ;
            return timeSinceLastExecution.TotalMinutes >= periodicity;
        }

        private async Task StartExecutionAsync(WebsiteRecord record, IExecutionRepository executionRepository, ICrawler crawler)
        {
            var execution = new Execution
            {
                Id = Guid.NewGuid(),
                WebsiteRecordId = record.Id,
                StartTime = DateTime.UtcNow,
                Status = ExecutionStatus.InProgress
            };

            await executionRepository.CreateAsync(execution);
            await crawler.CrawlAsync(record, execution);
        }

        public async Task ManualStartExecutionAsync(Guid websiteRecordId)
        {
            using var scope = _serviceProvider.CreateScope();
            var websiteRecordRepository = scope.ServiceProvider.GetRequiredService<IWebsiteRecordRepository>();
            var executionRepository = scope.ServiceProvider.GetRequiredService<IExecutionRepository>();
            var crawler = scope.ServiceProvider.GetRequiredService<ICrawler>();

            var record = await websiteRecordRepository.GetByIdAsync(websiteRecordId);
            if (record != null)
            {
                await StartExecutionAsync(record, executionRepository, crawler);
            }
        }
    }
}