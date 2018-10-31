using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.ETL
{
    public interface ITransformEngine : IPlugin
    {
        Task Transform(BuildContext context);
    }
}
