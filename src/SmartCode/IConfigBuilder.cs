using SmartCode.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode
{
    public interface IConfigBuilder
    {
        Project Build();
    }
}
