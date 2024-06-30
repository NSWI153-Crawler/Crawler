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

public class AppSchema : Schema
{
    public AppSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Query = serviceProvider.GetRequiredService<AppQuery>();
    }
}