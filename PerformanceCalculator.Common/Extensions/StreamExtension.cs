using System;
using System.IO;

namespace PerformanceCalculator.Common.Extensions
{
    public static class StreamExtension
    {
        public static string ConvertToBase64(this Stream stream)
        {
            var bytes = new Byte[(int) stream.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, (int) stream.Length);

            return Convert.ToBase64String(bytes);
        }
    }
}