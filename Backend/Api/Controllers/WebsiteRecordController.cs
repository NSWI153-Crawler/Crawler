using AutoMapper;
using Domain.Dtos;
using Domain.Dtos.Execution;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebsiteRecordController : ControllerBase
    {
        private readonly IWebsiteRecordRepository websiteRecordRepository;
        private readonly IExecutionRepository executionRepository;
        private readonly IMapper mapper;
        private readonly ICrawler crawler;

        public WebsiteRecordController(IWebsiteRecordRepository websiteRecordRepository, IExecutionRepository executionRepository, IMapper mapper, ICrawler crawler)
        {
            this.websiteRecordRepository = websiteRecordRepository;
            this.executionRepository = executionRepository;
            this.mapper = mapper;
            this.crawler = crawler;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var domain = await websiteRecordRepository.GetAllAsync();
            var result = new List<WebsiteRecordDto>();
            foreach(var websiteRecord in domain)
            {
                var lastExecution = await executionRepository.GetLastExecutionFromWebsiteRecord(websiteRecord.Id);
                var lastExecutionDto = mapper.Map<ExecutionDto?>(lastExecution);
                var websiteRecordDto = mapper.Map<WebsiteRecordDto>(websiteRecord);
                result.Add(WebsiteRecordDto.AddLastExecution(websiteRecordDto, lastExecutionDto));
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var domain = await websiteRecordRepository.GetByIdAsync(id);
            if (domain == null)
            {
                return NotFound();
            }
            var lastExecution = await executionRepository.GetLastExecutionFromWebsiteRecord(domain.Id);
            var lastExecutionDto = mapper.Map<ExecutionDto?>(lastExecution);
            var websiteRecordDto = mapper.Map<WebsiteRecordDto>(domain);

            return Ok(WebsiteRecordDto.AddLastExecution(websiteRecordDto, lastExecutionDto));
        }
        [HttpPost]
        public async Task<IActionResult> Post(CreateWebsiteRecordRequest request)
        {
            var domain = mapper.Map<WebsiteRecord>(request);
            domain = await websiteRecordRepository.CreateAsync(domain);
            return Ok(mapper.Map<WebsiteRecordDto>(domain));
        }
        [HttpPost]
        [Route("{id:Guid}")]

        public async Task<IActionResult> ManualCrawl(Guid id)
        {
            var websiteRecord = await websiteRecordRepository.GetByIdAsync(id);
            var execution = new Execution
            {
                Id = Guid.NewGuid(),
                WebsiteRecordId = id,
                StartTime = DateTime.UtcNow,
                Status = ExecutionStatus.InProgress,
                SitesCrawled = 0
            };

            await executionRepository.CreateAsync(execution);
            await crawler.CrawlAsync(websiteRecord, execution);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Put(Guid id, UpdateWebsiteRecordRequest request)
        {
            var domain = mapper.Map<WebsiteRecord>(request);
            domain = await websiteRecordRepository.UpdateAsync(id, domain);
            if (domain == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WebsiteRecordDto>(domain));
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var domain = await websiteRecordRepository.DeleteAsync(id);
            if (domain == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WebsiteRecordDto>(domain));
        }
    }
}
