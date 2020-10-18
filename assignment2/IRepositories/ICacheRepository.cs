using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace FHIR_FIT3077.IRepositories
{
    public interface ICacheRepository
    {
        void SetObject<T>(string key, T value);
        T GetObject<T>(string key);
        bool ExistObject<T>(string key);
        void Remove(string key);
    }
}
