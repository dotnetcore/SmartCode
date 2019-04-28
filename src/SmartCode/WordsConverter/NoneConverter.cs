using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.WordsConverter
{
    public class NoneConverter : IWordsConverter
    {
        public bool Initialized => true;

        public string Name => "None";

        public string Convert(IEnumerable<string> words)
        {
            return String.Join("", words);
        }

        public void Initialize(IDictionary<string, object> parameters)
        {
           
        }
    }
}
