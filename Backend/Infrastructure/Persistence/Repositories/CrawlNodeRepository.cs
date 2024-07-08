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
    public class CrawlNodeRepository : ICrawlNodeRepository
    {
        private readonly CrawlerDbContext dbContext;
        public CrawlNodeRepository(CrawlerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task AddAsync(CrawlNode crawlNode)
        {
            await dbContext.CrawlNodes.AddAsync(crawlNode);
            await dbContext.SaveChangesAsync();
        }

        public async Task<CrawlNode?> GetByUrlAndExecutionIdAsync(string url, Guid websiteRecordId)
        {
            return await dbContext.CrawlNodes.FirstOrDefaultAsync(x => x.Url == url && x.OwnerId == websiteRecordId);
        }

        public async Task UpdateAsync(CrawlNode node)
        {
            var existingNode = await dbContext.CrawlNodes.FirstOrDefaultAsync(x => x.Id == node.Id);

            existingNode.Owner = node.Owner;
            existingNode.OwnerId = node.OwnerId;
            existingNode.CrawlNodes = node.CrawlNodes;
            existingNode.Url = node.Url;
            existingNode.ParentNode = node.ParentNode;
            existingNode.ParentNodeId = node.ParentNodeId;
            existingNode.CrawlTime = node.CrawlTime;
            existingNode.Title = node.Title;
            await dbContext.SaveChangesAsync();
        }
    }
}
