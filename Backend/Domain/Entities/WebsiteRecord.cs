using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum State
    {
        Active,
        Inactive
    }
    public class WebsiteRecord : IEntity
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string BoundaryRegexp { get; set; }
        public int Periodicity { get; set; }
        public string Label { get; set; }
        public State State { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Execution> Executions { get; set; }
        public List<CrawlNode> CrawlNodes { get; set; }
    }
}
