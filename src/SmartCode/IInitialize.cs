using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public interface IInitialize
    {
        void Initialize(IDictionary<string, object> parameters);
    }
}
