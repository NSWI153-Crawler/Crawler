using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface ICrawlNodeRepository
    {
        Task AddAsync(CrawlNode crawlNode);
        Task<CrawlNode?> GetByUrlAndExecutionIdAsync(string url, Guid id);
        Task UpdateAsync(CrawlNode node);
    }
}
