using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.Execution
{
    public class UpdateExecutionRequest
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int SitesCrawled { get; set; }
        public ExecutionStatus Status { get; set; }
    }
}
