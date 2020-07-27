using System;

namespace Extensions {
    public static class LongExtensions {
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB" };
        public static string ToSizeSuffix(this long value) {
            if (value.Equals(0)) return "0 byte";
            if (value < 0) return $"-{ToSizeSuffix(-value)}";
            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));
            adjustedSize = (adjustedSize > 99) ? Math.Floor(adjustedSize) : (adjustedSize > 9) ? Math.Floor(adjustedSize * 10) / 10 : Math.Floor(adjustedSize * 100) / 100;
            return $"{adjustedSize} {SizeSuffixes[mag]}";
        }
    }
}