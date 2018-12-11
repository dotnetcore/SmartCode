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
            IWordsConverter converter = default;
            switch (wordsConverter.Type)
            {
                case "Camel":
                    {
                        converter = new CamelCaseConverter(); break;
                    }
                case "Pascal":
                    {
                        converter = new PascalCaseConverter(); break;
                    }
                case "Delimiter":
                    {
                        converter = new DelimiterConverter(); break;
                    }
                default:
                    {
                        return new NoneConverter();
                    }
            }
            converter.Initialize(wordsConverter.Paramters);
            return converter;
        }
    }
}
