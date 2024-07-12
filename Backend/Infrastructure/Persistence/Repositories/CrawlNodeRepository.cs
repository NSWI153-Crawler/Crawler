using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class CrawlNodeRepository : ICrawlNodeRepository
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CrawlNodeRepository> _logger;

        public CrawlNodeRepository(IServiceScopeFactory serviceScopeFactory, ILogger<CrawlNodeRepository> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task AddAsync(CrawlNode crawlNode)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                try
                {
                    var existingNode = await dbContext.CrawlNodes
                        .FirstOrDefaultAsync(x => x.Url == crawlNode.Url && x.OwnerId == crawlNode.OwnerId);

                    if (existingNode == null)
                    {
                        // Ensure ParentNodeId is valid
                        if (crawlNode.ParentNodeId.HasValue)
                        {
                            var parentExists = await dbContext.CrawlNodes.AnyAsync(x => x.Id == crawlNode.ParentNodeId.Value);
                            if (!parentExists)
                            {
                                crawlNode.ParentNodeId = null;
                            }
                        }

                        await dbContext.CrawlNodes.AddAsync(crawlNode);
                    }
                    else
                    {
                        // Update existing node
                        existingNode.CrawlTime = crawlNode.CrawlTime;
                        existingNode.Title = crawlNode.Title;
                        dbContext.CrawlNodes.Update(existingNode);
                    }

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding CrawlNode: {Url}", crawlNode.Url);
                    throw;
                }
            }
        }

        public async Task<CrawlNode?> GetByUrlAndExecutionIdAsync(string url, Guid websiteRecordId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                return await dbContext.CrawlNodes
                    .FirstOrDefaultAsync(x => x.Url == url && x.OwnerId == websiteRecordId);
            }
        }

        public async Task UpdateAsync(CrawlNode node)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                try
                {
                    var existingNode = await dbContext.CrawlNodes.FindAsync(node.Id);
                    if (existingNode != null)
                    {
                        dbContext.Entry(existingNode).CurrentValues.SetValues(node);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating CrawlNode: {Id}", node.Id);
                    throw;
                }
            }
        }

        public async Task AddRangeAsync(IEnumerable<CrawlNode> crawlNodes)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                try
                {
                    foreach (var node in crawlNodes)
                    {
                        var existingNode = await dbContext.CrawlNodes
                            .FirstOrDefaultAsync(x => x.Url == node.Url && x.OwnerId == node.OwnerId);

                        if (existingNode == null)
                        {
                            if (node.ParentNodeId.HasValue)
                            {
                                var parentExists = await dbContext.CrawlNodes.AnyAsync(x => x.Id == node.ParentNodeId.Value);
                                if (!parentExists)
                                {
                                    node.ParentNodeId = null;
                                }
                            }

                            await dbContext.CrawlNodes.AddAsync(node);
                            await dbContext.SaveChangesAsync();

                        }
                        else
                        {
                            existingNode.CrawlTime = node.CrawlTime;
                            existingNode.Title = node.Title;
                            dbContext.CrawlNodes.Update(existingNode);
                        }
                    }

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding range of CrawlNodes");
                    throw;
                }
            }
        }

        public async Task DeleteForWebsiteRecordAsync(Guid websiteRecordId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
                var existingNodes = await dbContext.CrawlNodes.Where(x => x.OwnerId == websiteRecordId).ToListAsync();
                foreach(var existingNode in existingNodes)
                {
                    dbContext.CrawlNodes.Remove(existingNode);
                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
}