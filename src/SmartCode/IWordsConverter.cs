using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public interface IWordsConverter : IPlugin
    {
        String Convert(IEnumerable<string> words);
    }
}
