using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.Generator.Entity
{
    public interface IConvertedName
    {
        String Name { get; set; }
        String ConvertedName { get; set; }
    }
}
