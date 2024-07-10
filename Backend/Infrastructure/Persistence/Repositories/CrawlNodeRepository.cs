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
    public class CrawlNodeRepository : ICrawlNodeRepository
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        public CrawlNodeRepository(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task AddAsync(CrawlNode crawlNode)
        {

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                await dbContext.CrawlNodes.AddAsync(crawlNode);
                await dbContext.SaveChangesAsync();

            }

        }

        public async Task<CrawlNode?> GetByUrlAndExecutionIdAsync(string url, Guid websiteRecordId)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();

                return await dbContext.CrawlNodes.FirstOrDefaultAsync(x => x.Url == url && x.OwnerId == websiteRecordId);

            }
        }

        public async Task UpdateAsync(CrawlNode node)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();

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
}
