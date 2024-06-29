using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.Crawling
{
    public class Crawler
    {
        public async Task CrawlAsync(WebsiteRecord record, Execution execution)
        {

        }

        private string ExtractTitle(string content)
        {
            // Implement title extraction logic
            return null;
        }

        private List<string> ExtractLinks(string content)
        {
            // Implement link extraction logic
            return null;
        }
    }
}