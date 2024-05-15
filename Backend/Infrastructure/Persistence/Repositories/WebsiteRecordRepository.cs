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
    public class WebsiteRecordRepository : IWebsiteRecordRepository
    {
        private readonly CrawlerDbContext dbContext;
        public WebsiteRecordRepository(CrawlerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<WebsiteRecord> CreateAsync(WebsiteRecord record)
        {
            await dbContext.WebsiteRecords.AddAsync(record);
            await dbContext.SaveChangesAsync();
            return record;
        }

        public async Task<WebsiteRecord?> DeleteAsync(Guid id)
        {
            var existingWebsiteRecord = await dbContext.WebsiteRecords.FirstOrDefaultAsync(x => x.Id == id);
            if (existingWebsiteRecord == null)
            {
                return null;
            }
            dbContext.WebsiteRecords.Remove(existingWebsiteRecord);
            await dbContext.SaveChangesAsync();
            return existingWebsiteRecord;
        }

        public async Task<List<WebsiteRecord>> GetAllAsync()
        {
            return await dbContext.WebsiteRecords.ToListAsync();
        }

        public async Task<WebsiteRecord?> GetByIdAsync(Guid id)
        {
            return await dbContext.WebsiteRecords.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<WebsiteRecord?> UpdateAsync(Guid id, WebsiteRecord record)
        {
            var existingWebsiteRecord = await dbContext.WebsiteRecords.FirstOrDefaultAsync(x => x.Id == id);
            if (existingWebsiteRecord == null)
            {
                return null;
            }
            existingWebsiteRecord.Url = record.Url;
            existingWebsiteRecord.Label = record.Label;
            existingWebsiteRecord.BoundaryRegexp = record.BoundaryRegexp;
            existingWebsiteRecord.Tags = record.Tags; // todo maybe check, that this does what it should
            existingWebsiteRecord.Periodicity = record.Periodicity;
            await dbContext.SaveChangesAsync();
            return existingWebsiteRecord;
        }
    }
}
