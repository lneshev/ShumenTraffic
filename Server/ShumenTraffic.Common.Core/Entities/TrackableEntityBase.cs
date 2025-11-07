using MoravianStar.Dao;
using System;

namespace ShumenTraffic.Common.Core.Entities
{
    public class TrackableEntityBase<TId> : EntityBase<TId>, ITrackableEntityBase
    {
        /// <summary>
        /// Timestamp when the record was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Timestamp when the record was last updated.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public override bool IsNew()
        {
            return base.IsNew() || CreatedAt == default;
        }
    }
}