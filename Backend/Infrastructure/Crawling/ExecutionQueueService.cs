using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Crawling
{
    public class ExecutionQueueService : BackgroundService
    {
        private readonly IWebsiteRecordRepository _websiteRecordRepository;
        private readonly ExecutionManager _executionManager;
        private readonly ILogger<ExecutionQueueService> _logger;

        public ExecutionQueueService(
            IWebsiteRecordRepository websiteRecordRepository,
            ExecutionManager executionManager,
            ILogger<ExecutionQueueService> logger)
        {
            _websiteRecordRepository = websiteRecordRepository;
            _executionManager = executionManager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking for website records to execute at: {time}", DateTimeOffset.Now);
                    
                    var activeRecords = await _websiteRecordRepository.GetActiveRecordsAsync();
                    foreach (var record in activeRecords)
                    {
                        if (await ShouldExecuteAsync(record))
                        {
                            try
                            {
                                await _executionManager.StartExecutionAsync(record.Id);
                                _logger.LogInformation("Started execution for website record: {recordId}", record.Id);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error starting execution for website record: {recordId}", record.Id);
                            }
                        }
                    }

                    // Delay for a minute before the next check
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the ExecutionQueueService");
                    // Delay for a short period before retrying in case of an error
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }

        private async Task<bool> ShouldExecuteAsync(WebsiteRecord record)
        {
            if (record.State != State.Active)
            {
                return false;
            }

            var lastExecution = await _executionManager.GetLastExecutionFromWebsiteRecord(record);
            if (lastExecution == null)
            {
                return true; // No previous execution, should execute
            }

            var timeSinceLastExecution = DateTime.UtcNow - lastExecution.EndTime;
            return timeSinceLastExecution.TotalMinutes >= record.Periodicity;
        }
    }
}
