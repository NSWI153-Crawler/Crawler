using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WebsiteRecord : IEntity
    {
        public Guid Id { get; set; }
        // todo types might change, lets see, how database will handle it
        public Uri Url { get; set; }
        public Regex BoundaryRegexp { get; set; }
        public TimeSpan Periodicity { get; set; }
        public string Label { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
