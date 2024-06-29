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

namespace Api;

public class ExecutionManager
{
    private readonly IWebsiteRecordRepository _websiteRecordRepository;
    private readonly IExecutionRepository _executionRepository;
    private readonly Crawler _crawler;
    
    public ExecutionManager(IWebsiteRecordRepository websiteRecordRepository, IExecutionRepository executionRepository, Crawler crawler)
    {
        _websiteRecordRepository = websiteRecordRepository;
        _executionRepository = executionRepository;
        _crawler = crawler;
    }
    
    public async Task StartExecutionAsync(WebsiteRecord record)
    {
       
    }
}