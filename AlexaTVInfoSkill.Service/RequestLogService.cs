using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlexaTVInfoSkill.Service.Model;

namespace AlexaTVInfoSkill.Service
{
    public class RequestLogService
    {
        public async Task<IEnumerable<RequestLog>> Get(DateTime? start = null, DateTime? end = null)
        {
            var items = await DocumentRepository<RequestLog>.GetItemsAsync();
            return items;
        }

        public async Task Add(RequestLog log)
        {
            await DocumentRepository<RequestLog>.CreateItemAsync(log);
        }
    }
}
