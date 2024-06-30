using GraphQL;
using GraphQL.Types;
using Api.GraphQL;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Entities;
using Domain.Common;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;

namespace Api.GraphQL;

public class AppQuery : ObjectGraphType
{
    public AppQuery(IWebsiteRecordRepository websiteRecordRepository, ICrawlNodeRepository crawlNodeRepository)
    {
        Field<NonNullGraphType<ListGraphType<NonNullGraphType<WebsiteRecordType>>>>("websites")
            .ResolveAsync(async context => await websiteRecordRepository.GetAllAsync());

        Field<NonNullGraphType<ListGraphType<NonNullGraphType<CrawlNodeType>>>>("nodes")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<ListGraphType<NonNullGraphType<IdGraphType>>>>
                {
                    Name = "webPages"
                }
            ))
            .ResolveAsync(async context =>
            {
                var webPageIds = context.GetArgument<List<Guid>>("webPages");
                var nodes = new List<CrawlNode>();
                foreach (var id in webPageIds)
                {
                    var record = await websiteRecordRepository.GetByIdAsync(id);
                    if (record != null) nodes.AddRange(record.CrawlNodes);
                }

                return nodes;
            });
    }
}
