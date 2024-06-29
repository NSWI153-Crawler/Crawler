using AutoMapper;
using Domain.Dtos;
using Domain.Dtos.Execution;
using Domain.Entities;
using Domain.Interfaces.Repositories;
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

        public WebsiteRecordController(IWebsiteRecordRepository websiteRecordRepository, IExecutionRepository executionRepository, IMapper mapper)
        {
            this.websiteRecordRepository = websiteRecordRepository;
            this.executionRepository = executionRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var domain = await websiteRecordRepository.GetAllAsync();
            var result = new List<WebsiteRecordDto>();
            foreach(var websiteRecord in domain)
            {
                var lastExecution = executionRepository.GetLastExecutionFromWebsiteRecord(websiteRecord.Id);
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
            var lastExecution = executionRepository.GetLastExecutionFromWebsiteRecord(domain.Id);
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
        [HttpPut]
        [Route("{id:Guid}")]
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
