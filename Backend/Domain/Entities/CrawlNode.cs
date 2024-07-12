using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CrawlNode : IEntity
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public DateTime CrawlTime { get; set; }
        public string Title { get; set; }
        public Guid OwnerId { get; set; }
        public WebsiteRecord Owner { get; set; }
        // parent
        public Guid? ParentNodeId { get; set; }
        public CrawlNode? ParentNode { get; set; }
        // children
        public List<CrawlNode> CrawlNodes { get; set; }

    }
}
