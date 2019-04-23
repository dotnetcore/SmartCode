using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartCode.Configuration;

namespace SmartCode
{
    public interface IOutput : IPlugin
    {
        Task Output(BuildContext context, Output output = null);
    }
}
