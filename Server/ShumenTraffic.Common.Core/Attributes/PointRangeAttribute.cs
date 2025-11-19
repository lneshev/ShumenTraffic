using NetTopologySuite.Geometries;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Common.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PointRangeAttribute : ValidationAttribute
    {
        private const double MinX = -180;
        private const double MaxX = 180;
        private const double MinY = -90;
        private const double MaxY = 90;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is not Point point)
            {
                return new ValidationResult($"{validationContext.DisplayName} must be of type {nameof(Point)} in order to apply attribute {nameof(PointRangeAttribute)}.");
            }

            if (point.X < MinX || point.X > MaxX)
            {
                return new ValidationResult($"X (Longitude) must be in range {MinX} to {MaxX}.");
            }

            if (point.Y < MinY || point.Y > MaxY)
            {
                return new ValidationResult($"Y (Latitude) must be in range {MinY} to {MaxY}.");
            }

            return ValidationResult.Success;
        }
    }
}