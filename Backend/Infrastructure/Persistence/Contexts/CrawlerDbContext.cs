using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Contexts
{
    public class CrawlerDbContext : DbContext
    {
        public CrawlerDbContext(DbContextOptions<CrawlerDbContext> dbContextOptions) : base(dbContextOptions) { }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<WebsiteRecord> WebsiteRecords { get; set; }
        public DbSet<Execution> Executions { get; set; }
        public DbSet<CrawlNode> CrawlNodes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // here you can seed data into the database
        }
    }
}
