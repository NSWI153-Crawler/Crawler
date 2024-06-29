using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class ExecutionRepository : IExecutionRepository
    {
        private readonly CrawlerDbContext dbContext;
        public ExecutionRepository(CrawlerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Execution> CreateAsync(Execution execution)
        {
            await dbContext.Executions.AddAsync(execution);
            await dbContext.SaveChangesAsync();
            return execution;
        }

        public async Task<Execution?> DeleteAsync(Guid id)
        {
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

        public async Task<List<Execution>> GetAllAsync()
        {
            return await dbContext.Executions.ToListAsync();
        }

        public async Task<IEnumerable<Execution?>> GetAllExecutionsFromWebsiteRecord(Guid websiteRecordId)
        {
            return await dbContext.Executions.Where(x => x.WebsiteRecord.Id == websiteRecordId).ToListAsync();
        }

        public async Task<Execution?> GetByIdAsync(Guid id)
        {
            return await dbContext.Executions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Execution?> GetLastExecutionFromWebsiteRecord(Guid websiteRecordId)
        {
            List<Execution> executions = await dbContext.Executions.Where(x => x.WebsiteRecord.Id == websiteRecordId).ToListAsync();
            if(executions.Count == 0)
            {
                // no executions yet for this website record
                return null;
            }
            return executions.OrderBy(x => x.EndTime).FirstOrDefault();
        }

        public async Task<Execution?> UpdateAsync(Guid id, Execution execution)
        {
            var existingExecution = await dbContext.Executions.FirstOrDefaultAsync(x => x.Id == id);
            if (existingExecution == null)
            {
                return null;
            }
            if (execution.StartTime > execution.EndTime) throw new ArgumentException("Start time must be sooner than end time.");
            existingExecution.StartTime = execution.StartTime;
            existingExecution.EndTime = execution.EndTime;
            existingExecution.SitesCrawled = execution.SitesCrawled;
            existingExecution.Status = execution.Status;
            await dbContext.SaveChangesAsync();
            return existingExecution;
        }
    }
}
