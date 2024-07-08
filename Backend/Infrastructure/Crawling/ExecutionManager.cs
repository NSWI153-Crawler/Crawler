using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Repositories;

using Infrastructure;
using Infrastructure.Crawling;

namespace Infrastructure.Crawling;

public class ExecutionManager
{
    private readonly IWebsiteRecordRepository _websiteRecordRepository;
    private readonly IExecutionRepository _executionRepository;
    private readonly ICrawler _crawler;

    public ExecutionManager(IWebsiteRecordRepository websiteRecordRepository, IExecutionRepository executionRepository, ICrawler crawler)
    {
        _websiteRecordRepository = websiteRecordRepository;
        _executionRepository = executionRepository;
        _crawler = crawler;
    }

    public async Task StartExecutionAsync(Guid websiteRecordId)
    {
        var websiteRecord = await _websiteRecordRepository.GetByIdAsync(websiteRecordId);
        if (websiteRecord == null)
        {
            throw new ArgumentException("Invalid WebsiteRecord ID", nameof(websiteRecordId));
        }

        if (websiteRecord.State != State.Active)
        {
            throw new InvalidOperationException("Cannot start execution for inactive WebsiteRecord");
        }

        var execution = new Execution
        {
            Id = Guid.NewGuid(),
            WebsiteRecordId = websiteRecordId,
            StartTime = DateTime.UtcNow,
            Status = ExecutionStatus.InProgress,
            SitesCrawled = 0
        };

        await _executionRepository.CreateAsync(execution);

        _ = Task.Run(async () =>
        {
            try
            {
                await _crawler.CrawlAsync(websiteRecord, execution);
            }
            catch (Exception ex)
            {
                execution.Status = ExecutionStatus.Failure;
                execution.EndTime = DateTime.UtcNow;
                await _executionRepository.UpdateAsync(execution.Id, execution);
            }
        });
    }

    public async Task<IEnumerable<Execution?>> GetExecutionsForWebsiteRecordAsync(Guid websiteRecordId)
    {
        return await _executionRepository.GetAllExecutionsFromWebsiteRecord(websiteRecordId);
    }

    public async Task<Execution?> GetExecutionByIdAsync(Guid executionId)
    {
        return await _executionRepository.GetByIdAsync(executionId);
    }

    public async Task SchedulePeriodicExecutionsAsync()
    {
        var activeRecords = await _websiteRecordRepository.GetActiveRecordsAsync();
        foreach (var record in activeRecords)
        {
            var lastExecution = await _executionRepository.GetLastExecutionFromWebsiteRecord(record.Id);
            if (lastExecution == null || ShouldStartNewExecution(lastExecution, record.Periodicity))
            {
                await StartExecutionAsync(record.Id);
            }
        }
    }

    private bool ShouldStartNewExecution(Execution lastExecution, int periodicity)
    {
        var timeSinceLastExecution = DateTime.UtcNow - lastExecution.EndTime;
        return timeSinceLastExecution.TotalMinutes >= periodicity;
    }

    public async Task<Execution?> GetLastExecutionFromWebsiteRecord(WebsiteRecord record)
    {
        return await _executionRepository.GetLastExecutionFromWebsiteRecord(record.Id);
    }
}