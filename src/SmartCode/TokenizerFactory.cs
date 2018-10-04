using SmartCode.Configuration;
using SmartCode.WordsConverter;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public class TokenizerFactory
    {
        public static ITokenizer Create(Tokenizer tokenizer)
        {
            DefaultTokenizer defaultTokenizer = new DefaultTokenizer();
            defaultTokenizer.Initialize(tokenizer.Paramters);
            return defaultTokenizer;
        }
    }
}
