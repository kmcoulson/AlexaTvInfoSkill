using System;
using System.Text;

namespace AlexaTVInfoSkill.Service
{
    public static class Extensions
    {
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            var parts = value.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();

            foreach (var part in parts)
            {
                sb.Append($"{part.Substring(0, 1).ToUpper()}{part.Substring(1).ToLower()} ");
            }
            return sb.ToString().Trim();
        }

        public static string TrimWords(this string value, string[] words)
        {
            if (string.IsNullOrEmpty(value)) return value;

            var newValue = value;

            foreach (var word in words)
            {
                if (newValue.StartsWith(word)) newValue = newValue.Substring(word.Length).Trim();
                if (newValue.EndsWith(word)) newValue = newValue.Substring(0, newValue.Length - word.Length).Trim();
            }

            return newValue.Trim();
        }
    }
}
