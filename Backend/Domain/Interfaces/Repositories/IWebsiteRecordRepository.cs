using Domain.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IWebsiteRecordRepository
    {
        public Task<List<WebsiteRecord>> GetAllAsync();
        public Task<WebsiteRecord?> GetByIdAsync(Guid id);
        public Task<WebsiteRecord> CreateAsync(WebsiteRecord websiteRecord);
        public Task<WebsiteRecord?> UpdateAsync(Guid id, WebsiteRecord websiteRecord);
        public Task<WebsiteRecord?> DeleteAsync(Guid id);
    }
}
