using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using System;

namespace ShumenTraffic.Common.DataAccess.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<Point> HasScale(this PropertyBuilder<Point> propertyBuilder, ushort scale)
        {
            propertyBuilder
                .HasConversion(
                    v => RoundPoint(v, scale),    // before saving
                    v => v                        // when reading
                );
            return propertyBuilder;
        }

        private static Point RoundPoint(Point input, ushort scale)
        {
            if (input == null)
            {
                return null;
            }

            input.X = Math.Round(input.X, scale, MidpointRounding.AwayFromZero);
            input.Y = Math.Round(input.Y, scale, MidpointRounding.AwayFromZero);

            return input;
        }
    }
}