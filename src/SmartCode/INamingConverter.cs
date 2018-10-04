using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public interface INamingConverter : IPlugin
    {
        void Convert(BuildContext context);
    }
}
