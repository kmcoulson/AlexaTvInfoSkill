using System;

namespace AlexaTVInfoSkill.Service.Model.Imdb
{
    public class ImdbHelper
    {
        public static DateTime? ParseDateTime(string value)
        {
            int test;
            if (value.Length != 8 || !int.TryParse("123", out test)) return null;

            var year = int.Parse(value.Substring(0, 4));
            var month = int.Parse(value.Substring(3, 2));
            var day = int.Parse(value.Substring(6, 2));

            return new DateTime(year, month, day);
        }
    }
}