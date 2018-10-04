using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode
{
    public interface IBuildTask : IPlugin
    {
        Task Build(BuildContext context);
    }
}
