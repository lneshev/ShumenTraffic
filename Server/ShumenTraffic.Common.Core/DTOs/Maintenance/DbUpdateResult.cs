using ShumenTraffic.Common.Core.Enums.Maintenance;
using System;

namespace ShumenTraffic.Common.Core.DTOs.Maintenance
{
    public class DbUpdateResult
    {
        public string Name { get; set; }
        public DbUpdateState State { get; set; }
        public Exception Exception { get; set; }
    }
}