using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net.Http;
using System.Text.RegularExpressions;
using Domain.Interfaces.Repositories;
using HtmlAgilityPack;

namespace Infrastructure.Crawling
{
    public class Crawler : ICrawler
    {
        private readonly HttpClient _httpClient = new();
        private readonly ICrawlNodeRepository _crawlNodeRepository;
        private readonly IExecutionRepository _executionRepository;

        public Crawler(ICrawlNodeRepository crawlNodeRepository, IExecutionRepository executionRepository)
        {
            _crawlNodeRepository = crawlNodeRepository;
            _executionRepository = executionRepository;
        }

        public async Task CrawlAsync(WebsiteRecord record, Execution execution)
        {
            try
            {
                execution.StartTime = DateTime.UtcNow;
                execution.Status = ExecutionStatus.InProgress;
                await _executionRepository.UpdateAsync(execution.Id, execution);

                var rootNode = await CrawlPageAsync(record.Url, record, execution, null);
                if (rootNode != null)
                {
                    await CrawlRecursivelyAsync(rootNode, record, execution);
                }

                execution.EndTime = DateTime.UtcNow;
                execution.Status = ExecutionStatus.Success;
            }
            catch (Exception ex)
            {
                execution.EndTime = DateTime.UtcNow;
                execution.Status = ExecutionStatus.Failure;
            }
            finally
            {
                await _executionRepository.UpdateAsync(execution.Id, execution);
            }
        }

        private async Task<CrawlNode> CrawlPageAsync(string url, WebsiteRecord record, Execution execution, CrawlNode parentNode)
        {
            if (!IsWithinBoundary(url, record.BoundaryRegexp))
                return null;

            var existingNode = await _crawlNodeRepository.GetByUrlAndExecutionIdAsync(url, record.Id);
            if (existingNode != null)
                return existingNode;

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var crawlNode = new CrawlNode
            {
                Id = Guid.NewGuid(),
                Url = url,
                CrawlTime = DateTime.UtcNow,
                Title = ExtractTitle(content),
                OwnerId = record.Id,
                Owner = record,
                ParentNodeId = parentNode?.Id ?? Guid.Empty,
                ParentNode = parentNode,
                CrawlNodes = new List<CrawlNode>()
            };
            await _crawlNodeRepository.AddAsync(crawlNode);
            execution.SitesCrawled++;
            await _executionRepository.UpdateAsync(execution.Id, execution);

            return crawlNode;
        }

        private async Task CrawlRecursivelyAsync(CrawlNode node, WebsiteRecord record, Execution execution)
        {
            if (node == null)
                return;

            var response = await _httpClient.GetAsync(node.Url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var links = ExtractLinks(content);

            foreach (var link in links)
            {
                var absoluteUrl = new Uri(new Uri(node.Url), link).AbsoluteUri;
                var childNode = await CrawlPageAsync(absoluteUrl, record, execution, node);
                if (childNode != null)
                {
                    node.CrawlNodes.Add(childNode);
                    await CrawlRecursivelyAsync(childNode, record, execution);
                }
            }

            await _crawlNodeRepository.UpdateAsync(node);
        }

        private string ExtractTitle(string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            return doc.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim() ?? string.Empty;
        }

        private List<string> ExtractLinks(string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
            return linkNodes?.Select(node => node.GetAttributeValue("href", string.Empty))
                            .Where(href => !string.IsNullOrEmpty(href))
                            .Distinct()
                            .ToList()
                   ?? new List<string>();
        }

        private bool IsWithinBoundary(string url, string boundaryRegexp)
        {
            return Regex.IsMatch(url, boundaryRegexp);
        }
    }

}