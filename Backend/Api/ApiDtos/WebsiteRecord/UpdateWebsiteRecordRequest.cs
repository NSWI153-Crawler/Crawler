using Api.ApiDtos.Tag;
using System.Text.RegularExpressions;

namespace Api.ApiDtos.WebsiteRecord
{
    public class UpdateWebsiteRecordRequest
    {
        public string Url { get; set; }
        public string BoundaryRegexp { get; set; }
        public int Periodicity { get; set; }
        public string Label { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
