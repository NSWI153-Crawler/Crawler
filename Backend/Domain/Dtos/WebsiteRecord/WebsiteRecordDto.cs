using Domain.Entities;
using System.Text.RegularExpressions;

namespace Domain.Dtos
{
    public class WebsiteRecordDto
    {
        public Guid Id { get; set; }
        // todo types might change, lets see, how database will handle it
        public string Url { get; set; }
        public string BoundaryRegexp { get; set; }
        public int Periodicity { get; set; }
        public string Label { get; set; }
        public State State { get; set; }

        public List<TagDto> Tags { get; set; }

    }
}
