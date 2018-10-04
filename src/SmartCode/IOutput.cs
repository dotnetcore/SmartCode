using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode
{
    public interface IOutput : IPlugin
    {
        Task Output(BuildContext context);
    }
}
