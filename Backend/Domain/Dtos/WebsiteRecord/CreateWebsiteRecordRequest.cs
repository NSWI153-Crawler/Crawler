﻿using System.Text.RegularExpressions;

namespace Domain.Dtos
{
    public class CreateWebsiteRecordRequest
    {
        public string Url { get; set; }
        public string BoundaryRegexp { get; set; }
        public int Periodicity { get; set; }
        public string Label { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}