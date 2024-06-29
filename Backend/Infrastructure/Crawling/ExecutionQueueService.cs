using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Crawling;

public class ExecutionQueueService : BackgroundService
{
    private readonly IWebsiteRecordRepository _websiteRecordRepository;
    private readonly ExecutionManager _executionManager;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                //var activeRecords = await _websiteRecordRepository.GetActiveRecordsAsync();
                    /*
                foreach (var record in activeRecords)
                {
                    if (ShouldExecute(record))
                    {
                        try
                        {
                            await _executionManager.StartExecutionAsync(record);
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                }
                */
                // Delay for a minute before the next check
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
    
    private bool ShouldExecute(WebsiteRecord record)
    {
        return false;
    }
}
