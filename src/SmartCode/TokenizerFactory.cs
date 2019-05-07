using SmartCode.Configuration;
using SmartCode.WordsConverter;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public class TokenizerFactory
    {
        public static ITokenizer Create(Tokenizer tokenizerConfig)
        {
            ITokenizer tokenizer;
            switch (tokenizerConfig.Type)
            {
                case "None":
                    {
                        tokenizer = new NoneTokenizer();
                        break;
                    }
                default:
                    {
                        tokenizer = new DefaultTokenizer();
                        break;
                    }
            }
            tokenizer.Initialize(tokenizerConfig.Parameters);
            return tokenizer;
        }
    }
}
