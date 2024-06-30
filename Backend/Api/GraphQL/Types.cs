using GraphQL;
using GraphQL.Types;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Entities;
using Domain.Common;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;

namespace Api.GraphQL;
public class WebsiteRecordType : ObjectGraphType<WebsiteRecord>
{
    public WebsiteRecordType()
    {
        Field("identifier", x => x.Id, type: typeof(IdGraphType));
        Field("label", x => x.Label);
        Field("url", x => x.Url);
        Field("regexp", x => x.BoundaryRegexp);
        Field<ListGraphType<StringGraphType>>("tags").Resolve(context => context.Source.Tags.Select(t => t.Name));
        Field<BooleanGraphType>("active").Resolve(context => context.Source.State == State.Active);
    }
}

public class CrawlNodeType : ObjectGraphType<CrawlNode>
{
    public CrawlNodeType()
    {
        Field("title", x => x.Title, nullable: true);
        Field("url", x => x.Url);
        Field<StringGraphType>("crawlTime").Resolve(context => context.Source.CrawlTime.ToString("O"));
        Field<ListGraphType<CrawlNodeType>>("links").Resolve(context => context.Source.CrawlNodes);
        Field<WebsiteRecordType>("owner").Resolve(context => context.Source.Owner);
    }
}