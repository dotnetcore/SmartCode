using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public interface IPluginManager
    {
        TPlugin Resolve<TPlugin>(String name = "") where TPlugin : IPlugin;
    }
}
