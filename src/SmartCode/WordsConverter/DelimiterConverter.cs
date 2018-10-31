using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartCode.WordsConverter
{
    public class DelimiterConverter : IWordsConverter
    {
        private const string DELIMITER = "Delimiter";
        private const string CONVERT_MODE = "Mode";
        private string _delimiter = "_";
        private ConvertMode _convertMode = ConvertMode.None;
        public DelimiterConverter(IDictionary<String, String> paramters)
        {
            if (paramters == null) { return; }
            if (!paramters.Value(DELIMITER, out _delimiter))
            {
                _delimiter = "_";
            }
            if (!paramters.Value(CONVERT_MODE, out _convertMode))
            {
                _convertMode = ConvertMode.None;
            }
        }
        public string Convert(IEnumerable<string> words)
        {
            switch (_convertMode)
            {
                case ConvertMode.AllLower:
                    {
                        return String.Join(_delimiter, words).ToLower();
                    }
                case ConvertMode.AllUpper:
                    {
                        return String.Join(_delimiter, words).ToUpper();
                    }
                case ConvertMode.FirstUpper:
                    {
                        var firstUpperWords = words.Select(word =>
                          {
                              string firstChar = word.Substring(0, 1).ToUpper();
                              string leftChar = word.Substring(1).ToLower();
                              return firstChar + leftChar;
                          });
                        return String.Join(_delimiter, firstUpperWords);
                    }
                case ConvertMode.None:
                    {
                        return String.Join(_delimiter, words);
                    }
                default:
                    {
                        throw new SmartCodeException($"can not support ConvertMode:{_convertMode}");
                    }
            }
        }
        public enum ConvertMode
        {
            None,
            AllLower,
            AllUpper,
            FirstUpper
        }
    }

}
