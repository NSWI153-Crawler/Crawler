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
   public class Crawler(ICrawlNodeRepository crawlNodeRepository, IExecutionRepository executionRepository)
   {
    private readonly HttpClient _httpClient = new();
    public async Task CrawlAsync(WebsiteRecord record, Execution execution)
    {
        try
        {
            execution.StartTime = DateTime.UtcNow;
            //TODO: add InProgress execution status
            execution.Status = ExecutionStatus.InProgress;
            await executionRepository.UpdateAsync(execution.Id,execution);

            var rootNode = await CrawlPageAsync(record.Url, record, execution, null);
            await CrawlRecursivelyAsync(rootNode, record, execution);

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
            await executionRepository.UpdateAsync(execution.Id,execution);
        }
    }

    private async Task<CrawlNode> CrawlPageAsync(string url, WebsiteRecord record, Execution execution, CrawlNode parentNode)
    {
        if (!IsWithinBoundary(url, record.BoundaryRegexp))
            return null;

        var existingNode = await crawlNodeRepository.GetByUrlAndExecutionIdAsync(url, execution.Id);
        if (existingNode != null)
            return existingNode;

        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        var crawlNode = new CrawlNode
        {
            Id = Guid.NewGuid(),
            Url = url,
            CrawlTime = (int)DateTime.UtcNow.Ticks,
            Title = ExtractTitle(content),
            OwnerId = record.Id,
            Owner = record,
            ParentNodeId = parentNode?.Id ?? Guid.Empty,
            ParentNode = parentNode,
            CrawlNodes = new List<CrawlNode>()
        };
        //TODO: implement AddAsync for CrawlNodes
        await crawlNodeRepository.AddAsync(crawlNode);
        execution.SitesCrawled++;
        await executionRepository.UpdateAsync(execution.Id,execution);

        return crawlNode;
    }

    private async Task CrawlRecursivelyAsync(CrawlNode node, WebsiteRecord record, Execution execution)
    {
        if (node == null)
            return;

        var response = await _httpClient.GetAsync(node.Url);
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
        //TODO: implement UpdateAsync for CrawlNodes
        await crawlNodeRepository.UpdateAsync(node);
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