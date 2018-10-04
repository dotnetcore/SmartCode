using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.WordsConverter
{
    public class NoneConverter : IWordsConverter
    {
        public string Convert(IEnumerable<string> words)
        {
            return String.Join("", words);
        }
    }
}
