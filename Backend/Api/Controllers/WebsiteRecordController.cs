using Api.ApiDtos.WebsiteRecord;
using AutoMapper;
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
        private readonly IWebsiteRecordRepository repository;
        private readonly IMapper mapper;

        public WebsiteRecordController(IWebsiteRecordRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var domain = await repository.GetAllAsync();
            return Ok(mapper.Map<List<WebsiteRecordDto>>(domain));
        }
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var domain = await repository.GetByIdAsync(id);
            if (domain == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WebsiteRecordDto>(domain));
        }
        [HttpPost]
        public async Task<IActionResult> Post(CreateWebsiteRecordRequest request)
        {
            var domain = mapper.Map<WebsiteRecord>(request);
            domain = await repository.CreateAsync(domain);
            return Ok(mapper.Map<WebsiteRecordDto>(domain));
        }
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Put(Guid id, UpdateWebsiteRecordRequest request)
        {
            var domain = mapper.Map<WebsiteRecord>(request);
            domain = await repository.UpdateAsync(id, domain);
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
            var domain = await repository.DeleteAsync(id);
            if (domain == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WebsiteRecordDto>(domain));
        }
    }
}
