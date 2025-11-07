using System;

namespace ShumenTraffic.Common.Core.Entities
{
    public interface ITrackableEntityBase
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}