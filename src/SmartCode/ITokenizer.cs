using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public interface ITokenizer : IPlugin
    {
        IEnumerable<String> Segment(string phrase);
    }
}
