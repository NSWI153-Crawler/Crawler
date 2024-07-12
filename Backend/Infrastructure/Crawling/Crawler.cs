using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Crawling
{
    public class Crawler : ICrawler
    {
        private readonly HttpClient _httpClient;
        private readonly ICrawlNodeRepository _crawlNodeRepository;
        private readonly IExecutionRepository _executionRepository;
        private readonly ILogger<Crawler> _logger;

        public Crawler(
            ICrawlNodeRepository crawlNodeRepository,
            IExecutionRepository executionRepository,
            IHttpClientFactory httpClientFactory,
            ILogger<Crawler> logger)
        {
            _crawlNodeRepository = crawlNodeRepository;
            _executionRepository = executionRepository;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task CrawlAsync(WebsiteRecord record, Execution execution)
        {
            try
            {
                execution.Status = ExecutionStatus.InProgress;
                await _executionRepository.UpdateAsync(execution.Id, execution);

                var crawledUrls = new HashSet<string>();
                var urlsToCrawl = new Queue<(string Url, Guid? ParentId)>();
                urlsToCrawl.Enqueue((record.Url, null));

                var crawlNodes = new List<CrawlNode>();

                while (urlsToCrawl.Count > 0)
                {
                    var (url, parentId) = urlsToCrawl.Dequeue();
                    if (crawledUrls.Contains(url) || !IsWithinBoundary(url, record.BoundaryRegexp))
                        continue;

                    var crawlNode = await CrawlPageAsync(url, record.Id, parentId);
                    crawledUrls.Add(url);
                    crawlNodes.Add(crawlNode);

                    foreach (var link in await ExtractLinksAsync(url))
                    {
                        if (!crawledUrls.Contains(link) && IsWithinBoundary(link, record.BoundaryRegexp))
                        {
                            urlsToCrawl.Enqueue((link, crawlNode.Id));
                        }
                    }

                    // Save nodes in batches to reduce database operations
                    if (crawlNodes.Count >= 100)
                    {
                        await _crawlNodeRepository.AddRangeAsync(crawlNodes);
                        crawlNodes.Clear();
                    }
                }

                // Save any remaining nodes
                if (crawlNodes.Any())
                {
                    await _crawlNodeRepository.AddRangeAsync(crawlNodes);
                }

                execution.EndTime = DateTime.UtcNow;
                execution.Status = ExecutionStatus.Success;
                execution.SitesCrawled = crawledUrls.Count;
                await _executionRepository.UpdateAsync(execution.Id, execution);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while crawling {WebsiteRecordId}", record.Id);
                execution.EndTime = DateTime.UtcNow;
                execution.Status = ExecutionStatus.Failure;
                await _executionRepository.UpdateAsync(execution.Id, execution);
            }
        }

        private async Task<CrawlNode> CrawlPageAsync(string url, Guid ownerId, Guid? parentId)
        {
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            return new CrawlNode
            {
                Id = Guid.NewGuid(),
                Url = url,
                CrawlTime = DateTime.UtcNow,
                Title = ExtractTitle(content),
                OwnerId = ownerId,
                ParentNodeId = parentId
            };
        }

        private string ExtractTitle(string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            return doc.DocumentNode.SelectSingleNode("//title")?.InnerText.Trim() ?? string.Empty;
        }

        private async Task<IEnumerable<string>> ExtractLinksAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
            return linkNodes?.Select(node => new Uri(new Uri(url), node.GetAttributeValue("href", "")).ToString())
                             .Distinct()
                             ?? Enumerable.Empty<string>();
        }

        private bool IsWithinBoundary(string url, string boundaryRegexp)
        {
            return Regex.IsMatch(url, boundaryRegexp, RegexOptions.IgnoreCase);
        }
    }
}