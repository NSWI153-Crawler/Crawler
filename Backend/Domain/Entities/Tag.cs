﻿using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Tag : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid WebsiteRecordId { get; set; }
        public WebsiteRecord WebsiteRecord { get; set; }
    }
}
