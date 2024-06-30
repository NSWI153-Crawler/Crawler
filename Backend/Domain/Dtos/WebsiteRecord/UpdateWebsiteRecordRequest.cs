using Domain.Entities;
using System.Text.RegularExpressions;

namespace Domain.Dtos
{
    public class UpdateWebsiteRecordRequest
    {
        public string Url { get; set; }
        public string BoundaryRegexp { get; set; }
        public int Periodicity { get; set; }
        public string Label { get; set; }
        public State State { get; set; }

        public List<TagDto> Tags { get; set; }
    }
}
