using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ExecutionRepository : IExecutionRepository
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        public ExecutionRepository(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<Execution> CreateAsync(Execution execution)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();

                await dbContext.Executions.AddAsync(execution);
                await dbContext.SaveChangesAsync();
                return execution;
            }

        }

        public async Task<Execution?> DeleteAsync(Guid id)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                var existingExecution = await dbContext.Executions
                     .FirstOrDefaultAsync(x => x.Id == id);
                if (existingExecution == null)
                {
                    return null;
                }
                dbContext.Executions.Remove(existingExecution);
                await dbContext.SaveChangesAsync();
                return existingExecution;
            }
        }

        public async Task<List<Execution>> GetAllAsync()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                return await dbContext.Executions.Include(x => x.WebsiteRecord).ToListAsync();
            }
        }

        public async Task<IEnumerable<Execution?>> GetAllExecutionsFromWebsiteRecord(Guid websiteRecordId)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                return await dbContext.Executions.Where(x => x.WebsiteRecord.Id == websiteRecordId).ToListAsync();
            }
        }

        public async Task<Execution?> GetByIdAsync(Guid id)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                return await dbContext.Executions.Include(x => x.WebsiteRecord).FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public async Task<Execution?> GetLastExecutionFromWebsiteRecord(Guid websiteRecordId)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                List<Execution> executions = await dbContext.Executions.Where(x => x.WebsiteRecord.Id == websiteRecordId).ToListAsync();
                if (executions.Count == 0)
                {
                    // no executions yet for this website record
                    return null;
                }
                return executions.OrderBy(x => x.EndTime).FirstOrDefault();
            }
        }

        public async Task<Execution?> UpdateAsync(Guid id, Execution execution)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                var existingExecution = await dbContext.Executions.FirstOrDefaultAsync(x => x.Id == id);
                if (existingExecution == null)
                {
                    return null;
                }
                if (execution.StartTime > execution.EndTime) ;
                existingExecution.StartTime = execution.StartTime;
                existingExecution.EndTime = execution.EndTime;
                existingExecution.SitesCrawled = execution.SitesCrawled;
                existingExecution.Status = execution.Status;
                await dbContext.SaveChangesAsync();
                return existingExecution;
            }
        }
    }
}
