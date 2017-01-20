using System;
using System.Text;

namespace AlexaTVInfoSkill.Service
{
    public static class Extensions
    {
        public static string ToCamelCase(this string value)
        {
            if (value == null) return null;

            var parts = value.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();

            foreach (var part in parts)
            {
                sb.Append($"{part.Substring(0, 1).ToUpper()}{part.Substring(1).ToLower()} ");
            }
            return sb.ToString().Trim();
        }
    }
}
