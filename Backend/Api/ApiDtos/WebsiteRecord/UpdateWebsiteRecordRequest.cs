using Api.ApiDtos.Tag;
using System.Text.RegularExpressions;

namespace Api.ApiDtos.WebsiteRecord
{
    public class UpdateWebsiteRecordRequest
    {
        public Uri Url { get; set; }
        public Regex BoundaryRegexp { get; set; }
        public TimeSpan Periodicity { get; set; }
        public string Label { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
