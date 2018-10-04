using SmartCode.WordsConverter;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartCode.Tests
{
    public class NamingConverter_Test
    {
        ITokenizer _defaultTokenizer = new DefaultTokenizer();
        IWordsConverter _namingConverter = new PascalCaseConverter();
        public NamingConverter_Test()
        {
            var paramters = new Dictionary<String, String> {
                {DefaultTokenizer.IGNORE_PREFIX_KEY,"T_" },
                { DefaultTokenizer.DELIMITER_KEY,"_"}
            };
            _defaultTokenizer.Initialize(paramters);
        }
        [Fact]
        public void Convert()
        {
            string phrase = "T_smart_sql_good";
            var words = _defaultTokenizer.Segment(phrase);
            var convertedNaming = _namingConverter.Convert(words);
            Assert.Equal("SmartSqlGood", convertedNaming);
        }
    }
}
