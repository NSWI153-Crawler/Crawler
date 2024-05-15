using Api.ApiDtos.Tag;
using Api.ApiDtos.WebsiteRecord;
using AutoMapper;
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
        }
    }
}
