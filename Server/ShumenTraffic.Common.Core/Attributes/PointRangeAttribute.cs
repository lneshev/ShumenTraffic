using NetTopologySuite.Geometries;
using ShumenTraffic.Common.Core.Extensions;
using ShumenTraffic.Common.Core.Resources;
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
                return new ValidationResult(string.Format(Strings.PropertyMustBeOfTypeInOrderToApplyAttribute, validationContext.DisplayName, nameof(Point), nameof(PointRangeAttribute)));
            }

            if (point.GetLongitude() < MinX || point.GetLongitude() > MaxX)
            {
                return new ValidationResult(string.Format(Strings.XLongitudeMustBeInRange, MinX, MaxX));
            }

            if (point.GetLatitude() < MinY || point.GetLatitude() > MaxY)
            {
                return new ValidationResult(string.Format(Strings.YLatitudeMustBeInRange, MinY, MaxY));
            }

            return ValidationResult.Success;
        }
    }
}