using AutoMapper;
using Domain.Dtos.Execution;
using Domain.Dtos;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using Domain.Entities;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecutionController : ControllerBase
    {
        private readonly IExecutionRepository repository;
        private readonly IMapper mapper;

        public ExecutionController(IExecutionRepository executionRepository, IMapper mapper)
        {
            this.repository = executionRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var domain = await repository.GetAllAsync();
            return Ok(mapper.Map<List<ExecutionDto>>(domain));
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
            return Ok(mapper.Map<ExecutionDto>(domain));
        }
        [HttpPost]
        public async Task<IActionResult> Post(CreateExecutionRequest request)
        {
            var domain = mapper.Map<Execution>(request);
            domain = await repository.CreateAsync(domain);
            return Ok(mapper.Map<ExecutionDto>(domain));
        }
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Put(Guid id, UpdateExecutionRequest request)
        {
            var domain = mapper.Map<Execution>(request);
            domain = await repository.UpdateAsync(id, domain);
            if (domain == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<ExecutionDto>(domain));
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
            return Ok(mapper.Map<ExecutionDto>(domain));
        }
    }
}
