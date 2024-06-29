using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IExecutionRepository
    {
        public Task<List<Execution>> GetAllAsync();
        public Task<Execution?> GetByIdAsync(Guid id);
        public Task<Execution> CreateAsync(Execution execution);
        public Task<Execution?> UpdateAsync(Guid id, Execution execution);
        public Task<Execution?> DeleteAsync(Guid id);
        public Task<Execution?> GetLastExecutionFromWebsiteRecord(Guid websiteRecordId);
        public Task<IEnumerable<Execution?>> GetAllExecutionsFromWebsiteRecord(Guid websiteRecordId);
        
    }
}
