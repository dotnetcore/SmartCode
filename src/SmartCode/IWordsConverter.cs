using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public interface IWordsConverter
    {
        String Convert(IEnumerable<string> words);
    }
}
