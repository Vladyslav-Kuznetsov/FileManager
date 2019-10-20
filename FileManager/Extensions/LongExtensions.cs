using System;


namespace FileManager.Extensions
{
    public static class LongExtensions
    {
        public static string ToViewableSize(this long size)
        {
            var result = string.Empty;

            if (size > Math.Pow(2, 30))
            {
                result = $"{Math.Round(size / Math.Pow(2, 30), 2)} GB";
            }
            else if (size > Math.Pow(2, 20))
            {
                result = $"{Math.Round(size / Math.Pow(2, 20), 2)} MB";
            }
            else if (size > Math.Pow(2, 10))
            {
                result = $"{Math.Round(size / Math.Pow(2, 10), 2)} KB";
            }
            else
            {
                result = $"{size} Byte";
            }

            return result;
        }
    }
}
