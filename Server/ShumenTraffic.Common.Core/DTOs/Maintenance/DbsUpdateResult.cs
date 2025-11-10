using ShumenTraffic.Common.Core.Enums.Maintenance;
using System.Collections.Generic;

namespace ShumenTraffic.Common.Core.DTOs.Maintenance
{
    public class DbsUpdateResult
    {
        public DbsUpdateResult()
        {
            Results = new List<DbUpdateResult>();
        }

        public DbsUpdateState State { get; set; }
        public List<DbUpdateResult> Results { get; set; }
    }
}