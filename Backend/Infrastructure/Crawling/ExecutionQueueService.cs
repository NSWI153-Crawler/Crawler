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
using Microsoft.Extensions.DependencyInjection;
namespace Infrastructure.Crawling
{
    public class ExecutionQueueService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ExecutionQueueService> _logger;

        public ExecutionQueueService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ExecutionQueueService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Checking for website records to execute at: {time}", DateTimeOffset.Now);
                    
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var websiteRecordRepository = scope.ServiceProvider.GetRequiredService<IWebsiteRecordRepository>();
                        var executionManager = scope.ServiceProvider.GetRequiredService<ExecutionManager>();

                        var activeRecords = await websiteRecordRepository.GetActiveRecordsAsync();
                        foreach (var record in activeRecords)
                        {
                            if (await ShouldExecuteAsync(record, executionManager))
                            {
                                try
                                {
                                    await executionManager.StartExecutionAsync(record.Id);
                                    _logger.LogInformation("Started execution for website record: {recordId}", record.Id);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error starting execution for website record: {recordId}", record.Id);
                                }
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

        private async Task<bool> ShouldExecuteAsync(WebsiteRecord record, ExecutionManager executionManager)
        {
            if (record.State != State.Active)
            {
                return false;
            }

            var lastExecution = await executionManager.GetLastExecutionFromWebsiteRecord(record);
            if (lastExecution == null)
            {
                return true; // No previous execution, should execute
            }

            var timeSinceLastExecution = DateTime.UtcNow - lastExecution.EndTime;
            return timeSinceLastExecution.TotalMinutes >= record.Periodicity;
        }
    }
}

