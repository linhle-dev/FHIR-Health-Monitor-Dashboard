using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using FHIR_FIT3077.IRepositories;
using FHIR_FIT3077.IRepository;
using FHIR_FIT3077.Models;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace FHIR_FIT3077.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        
        private readonly IDistributedCache _distributedCache;

        public CacheRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [Obsolete]
        public void SetObject<T>(string key, T value)
        {
            var res = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            _distributedCache.SetString(key, res);
        }

        public T GetObject<T>(string key)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            var value =  _distributedCache.GetString(key);
            if (value != null)
            {
                var result = JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                return result;

            }
            else
            {
                return default(T);
            }
        }

        public bool ExistObject<T>(string key)
        {
            bool value;
            var obj = GetObject<T>(key);
            if (string.IsNullOrEmpty(_distributedCache.GetString(key)) || obj == null)
            {
                value = false;
            }
            else
            {
                value = true;
            } ;
            return value;
        }

        public void Remove(string key)
        {
            _distributedCache.Remove(key);
        }
    }
}
