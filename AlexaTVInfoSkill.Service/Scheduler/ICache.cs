using System;

namespace AlexaTVInfoSkill.Service.Scheduler
{
    public interface ICache<T> where T : class
    {
        void Put(string key, T value, TimeSpan duration, Action<string, T> onExpiration = null);

        T Get(string key);

        void Remove(string key);
    }
}
