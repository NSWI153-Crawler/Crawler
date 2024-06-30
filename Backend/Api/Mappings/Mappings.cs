using AutoMapper;
using Domain.Dtos;
using Domain.Dtos.Execution;
using Domain.Entities;

namespace Api.Mappings
{
    public class Mappings : Profile
    {
        public Mappings() {
            CreateMap<WebsiteRecord, WebsiteRecordDto>().ReverseMap();
            CreateMap<WebsiteRecord, CreateWebsiteRecordRequest>().ReverseMap();
            CreateMap<WebsiteRecord, UpdateWebsiteRecordRequest>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<Execution, ExecutionDto>().ReverseMap();
            CreateMap<Execution, CreateExecutionRequest>().ReverseMap();
            CreateMap<Execution, UpdateExecutionRequest>().ReverseMap();
        }
    }
}
