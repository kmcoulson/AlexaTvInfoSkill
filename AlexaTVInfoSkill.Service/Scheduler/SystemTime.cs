using System;

namespace AlexaTVInfoSkill.Service.Scheduler
{
    public struct SystemTime
    {
        private static readonly Func<DateTime> _default = () => DateTime.Now;

        private static Func<DateTime> _now = _default;

        public static DateTime Now
        {
            get { return _now(); }
            set { _now = () => value; }
        }

        public static DateTime UtcNow
        {
            get { return TimeZoneInfo.ConvertTimeToUtc(Now); }
        }

        public static void Default()
        {
            _now = _default;
        }
    }
}
