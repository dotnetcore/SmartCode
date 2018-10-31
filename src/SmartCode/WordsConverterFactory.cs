using SmartCode.Configuration;
using SmartCode.WordsConverter;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public class WordsConverterFactory
    {
        public static IWordsConverter Create(Configuration.WordsConverter wordsConverter)
        {
            switch (wordsConverter.Type)
            {
                case "Camel":
                    {
                        return new CamelCaseConverter();
                    }
                case "Pascal":
                    {
                        return new PascalCaseConverter();
                    }
                case "Delimiter":
                    {
                        return new DelimiterConverter(wordsConverter.Paramters);
                    }
                default:
                    {
                        return new NoneConverter();
                    }
            }
        }
    }
}
