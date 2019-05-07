using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.WordsConverter
{
    public class NoneTokenizer : ITokenizer
    {
        public void Initialize(IDictionary<string, object> parameters)
        {

        }

        public bool Initialized => true;
        public string Name => "None";
        public IEnumerable<string> Segment(string phrase)
        {
            return new[] { phrase };
        }
    }
}
