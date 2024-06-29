using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum ExecutionStatus
    {
        Success,
        Failure,
        InProgress
    }
    public class Execution : IEntity
    {
        public Guid Id { get; set; }
        public DateTime StartTime {  get; set; }
        public DateTime EndTime { get; set; }
        public int SitesCrawled { get; set; }
        public ExecutionStatus Status { get; set; }
        public Guid WebsiteRecordId { get; set; }
        public WebsiteRecord WebsiteRecord { get; set; }
    }
}
