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
        Task AddRangeAsync(IEnumerable<CrawlNode> crawlNodes);
        Task<CrawlNode?> GetByUrlAndExecutionIdAsync(string url, Guid websiteRecordId);
        Task UpdateAsync(CrawlNode node);
    }
}
