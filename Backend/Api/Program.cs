using Api;
using Api.Mappings;
using Domain.Interfaces.Repositories;
using Infrastructure;
using DotNetEnv;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Crawling;
using Api.GraphQL;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
DotNetEnv.Env.Load();

builder.Services.AddInfrastructure(); // This method should register infrastructure services


// Register other necessary services for ExecutionQueueService
builder.Services.AddScoped<IWebsiteRecordRepository, WebsiteRecordRepository>();
builder.Services.AddScoped<ExecutionManager>();
// Register the ExecutionQueueService
builder.Services.AddHostedService<ExecutionQueueService>();

//graphql service
builder.Services.AddScoped<AppQuery>();
builder.Services.AddScoped<WebsiteRecordType>();
builder.Services.AddScoped<CrawlNodeType>();
builder.Services.AddScoped<ISchema, AppSchema>();
builder.Services.AddGraphQL(builder => builder
    .AddSystemTextJson() 
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Mappings));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseGraphQL<ISchema>();
app.UseGraphQLPlayground("/playground", new GraphQL.Server.Ui.Playground.PlaygroundOptions());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();