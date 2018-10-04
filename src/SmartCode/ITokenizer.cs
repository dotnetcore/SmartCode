using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public interface ITokenizer : IInitialize
    {
        IEnumerable<String> Segment(string phrase);
    }
}
