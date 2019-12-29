using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Lib.Common.Interfaces
{
    public interface ICacheProvider
    {
        bool Get<T>(object key, out T value);
        T Set<T>(object key, T value);
        void Reset();
    }
}
