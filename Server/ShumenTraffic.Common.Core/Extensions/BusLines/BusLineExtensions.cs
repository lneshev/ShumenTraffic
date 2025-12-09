using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Resources;
using System;
using System.Linq;

namespace ShumenTraffic.Common.Core.Extensions.BusLines
{
    public static class BusLineExtensions
    {
        public static string GenerateBusLineNumberSortKey(this BusLine busLine)
        {
            return GenerateBusLineNumberSortKey(busLine.LineNumber);
        }

        private static string GenerateBusLineNumberSortKey(string lineNumber)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(lineNumber);
            string result = null;
            int numberWidth = 10;
            char delimeter = '|';

            if (char.IsDigit(lineNumber[0]))
            {
                // "0|0000000001|A
                var number = string.Join(string.Empty, lineNumber.TakeWhile(char.IsDigit));
                if (number.Length > numberWidth)
                {
                    throw new ArgumentException(string.Format(Strings.TheNumericPartInParameterShouldBeNoMoreThanXDigits, lineNumber, numberWidth), lineNumber);
                }

                var suffix = string.Join(string.Empty, lineNumber.SkipWhile(char.IsDigit));
                result = "0" + delimeter + number.PadLeft(numberWidth, '0') + delimeter + suffix;
            }
            else
            {
                // "2|BusStopName"
                result = "2" + delimeter + lineNumber;
            }

            return result;
        }
    }
}