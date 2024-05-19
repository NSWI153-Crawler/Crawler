using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Crawling
{
    public class Crawler : ICrawler
    {
        public Task<Execution> CrawlWebsiteRecord(WebsiteRecord websiteRecord)
        {
            throw new NotImplementedException();
        }

    }
}
