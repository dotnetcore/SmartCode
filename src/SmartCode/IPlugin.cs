using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public interface IPlugin : IInitialize
    {
        bool Initialized { get; }
        String Name { get; }
    }
}
