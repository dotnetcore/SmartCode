using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.Configuration
{
    public class NamingConverter
    {
        public static NamingConverter Default = new NamingConverter
        {
            Table = TokenizerMapConverter.Default,
            Column = TokenizerMapConverter.Default,
            View = TokenizerMapConverter.Default
        };
        public TokenizerMapConverter Table { get; set; }
        public TokenizerMapConverter View { get; set; }
        public TokenizerMapConverter Column { get; set; }
    }

    public class TokenizerMapConverter
    {
        public static TokenizerMapConverter Default = new TokenizerMapConverter
        {
            Tokenizer = Tokenizer.Default,
            Converter = WordsConverter.Default
        };
        public Tokenizer Tokenizer { get; set; }
        public WordsConverter Converter { get; set; }
    }

    /// <summary>
    /// 分词器
    /// </summary>
    public class Tokenizer
    {
        public static Tokenizer Default = new Tokenizer
        {
            Type = "Default",
            Parameters = new Dictionary<String, object>()
        };
        public String Type { get; set; }
        public IDictionary<String, object> Parameters { get; set; }
    }
    /// <summary>
    /// 词转换器
    /// </summary>
    public class WordsConverter
    {
        public static WordsConverter Default = new WordsConverter
        {
            Type = "Default",
            Parameters = new Dictionary<String, Object>()
        };
        public String Type { get; set; }
        public IDictionary<String, object> Parameters { get; set; }
    }
}
