using Domain.Dtos.Execution;
using Domain.Entities;
using System.Text.RegularExpressions;

namespace Domain.Dtos
{
    public class WebsiteRecordDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string BoundaryRegexp { get; set; }
        public int Periodicity { get; set; }
        public string Label { get; set; }
        public State State { get; set; }

        public List<TagDto> Tags { get; set; }
        public ExecutionDto? LastExecution { get; set; }
        public static WebsiteRecordDto AddLastExecution(WebsiteRecordDto websiteRecord, ExecutionDto? execution)
        {
            return new WebsiteRecordDto
            {
                Id = websiteRecord.Id,
                Url = websiteRecord.Url,
                BoundaryRegexp = websiteRecord.BoundaryRegexp,
                Periodicity = websiteRecord.Periodicity,
                Label = websiteRecord.Label,
                State = websiteRecord.State,
                Tags = websiteRecord.Tags,
                LastExecution = execution
            };
        }

    }
}
